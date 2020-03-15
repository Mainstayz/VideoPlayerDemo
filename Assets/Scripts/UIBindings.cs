using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBindings : MonoBehaviour
{
    public RawImage displayLayer;
    public InputField inputField;
    public Button prepareBtn;
    public Button playBtn;
    public Button pauseBtn;
    public Button stopBtn;


    private VideoPlayerHelper mVideoPlayer = null;
    private VideoPlayerHelper.MediaState mCurrentState = VideoPlayerHelper.MediaState.NOT_READY;

    private IntPtr texPtr;
    private bool mIsInited = false;
    private bool mIsPrepared = false;

    private bool mAppPaused = false;
    private Texture2D mVideoTexture = null;


    // Start is called before the first frame update
    void Start()
    {
        // 添加监听
        inputField.onEndEdit.AddListener(TextFieldOnEndEdit);
        prepareBtn.onClick.AddListener(Prepare);
        playBtn.onClick.AddListener(Play);
        pauseBtn.onClick.AddListener(Pause);
        stopBtn.onClick.AddListener(Stop);


        float width = displayLayer.rectTransform.rect.width;
        float height = displayLayer.rectTransform.rect.height;

        mVideoTexture = new Texture2D((int)width, (int)height, TextureFormat.RGB565, false);
        mVideoTexture.filterMode = FilterMode.Bilinear;
        mVideoTexture.wrapMode = TextureWrapMode.Clamp;

        displayLayer.texture = mVideoTexture;
                
        //// 初始化播放器
        mVideoPlayer = gameObject.AddComponent<VideoPlayerHelper>();
        mVideoPlayer.Init();

        // 注意纹理的获取时机,mVideoTexture被负载到gameobject才能获取到texturePtr
        texPtr = mVideoTexture.GetNativeTexturePtr();
        mVideoPlayer.SetVideoTexturePtr(texPtr);


        HandleStateChange(VideoPlayerHelper.MediaState.NOT_READY);
        mCurrentState = VideoPlayerHelper.MediaState.NOT_READY;


    }

    void TextFieldOnEndEdit(string arg0)
    {
        mVideoPlayer.Unload();
        mIsInited = false;
    }

    void Prepare()
    {
        string url = inputField.text;
        if (mVideoPlayer.Load(url, false, -1) == false)
        {
            Debug.Log("Could not initialize video player");
            HandleStateChange(VideoPlayerHelper.MediaState.ERROR);
            this.enabled = false;
            return;
        }
        mIsInited = true;
    }

    void Play()
    {
        mVideoPlayer.Play(-1);
    }

    void Pause()
    {
        mVideoPlayer.Pause();
    }

    void Stop()
    {
        mVideoPlayer.Stop();
    }

    private void OnRenderObject()
    {

      
        if (mAppPaused) return;
        if (!mIsInited) return;
        if (!mIsPrepared)
        {
            Debug.Log("!mIsPrepared....");

            VideoPlayerHelper.MediaState state = mVideoPlayer.GetStatus();

            if (state == VideoPlayerHelper.MediaState.ERROR)
            {
                Debug.Log("Could not load video ");
                HandleStateChange(VideoPlayerHelper.MediaState.ERROR);
                this.enabled = false;
            }

            else if (state < VideoPlayerHelper.MediaState.NOT_READY)
            {
                // Video player is ready

                // Initialize the video texture
                // Pass the video texture id to the video player
                // TODO: GetNativeTexturePtr() call needs to be moved to Awake method to work with Oculus SDK if MT rendering is enabled
            
          
                // Get the video width and height
                int videoWidth = mVideoPlayer.GetVideoWidth();
                int videoHeight = mVideoPlayer.GetVideoHeight();

                if (videoWidth > 0 && videoHeight > 0)
                {
                    // Scale the video plane to match the video aspect ratio
                    float aspect = videoHeight / (float)videoWidth;

                    // Flip the plane as the video texture is mirrored on the horizontal
                    transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f * aspect);
                }
                // Video is prepared, ready for playback
                mIsPrepared = true;
                Debug.Log("Video is prepared, ready for playback");
            }
           
        }
        else
        {

            VideoPlayerHelper.MediaState state = mVideoPlayer.UpdateVideoData();

            if (state == VideoPlayerHelper.MediaState.PLAYING)
            {
                GL.InvalidateState();
            }

            // Check for playback state change
            if (state != mCurrentState)
            {
                HandleStateChange(state);
                mCurrentState = state;
            }
        }


    }

    void OnDestroy()
    {
        // Deinit the video
        mVideoPlayer.Deinit();
    }

    private void HandleStateChange(VideoPlayerHelper.MediaState newState)
    {
        // If the movie is playing or paused render the video texture
        // Otherwise render the keyframe

        // Display the appropriate icon, or disable if not needed
        switch (newState)
        {
            case VideoPlayerHelper.MediaState.PLAYING:
                Debug.Log("PLAYING");
                break;
            case VideoPlayerHelper.MediaState.READY:
                Debug.Log("READY");
                break;

            case VideoPlayerHelper.MediaState.REACHED_END:
                Debug.Log("REACHED_END");
                break;

            case VideoPlayerHelper.MediaState.PAUSED:
                Debug.Log("PAUSED");
                break;

            case VideoPlayerHelper.MediaState.STOPPED:
                Debug.Log("STOPPED");
                break;

            case VideoPlayerHelper.MediaState.NOT_READY:
                Debug.Log("NOT_READY");
                break;

            case VideoPlayerHelper.MediaState.ERROR:
                Debug.Log("ERROR");
                break;

            default:
                break;
        }
    }
}
