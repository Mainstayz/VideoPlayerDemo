using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBindings : MonoBehaviour
{
    // UI
    public RawImage displayLayer;
    public InputField inputField;
   
    public Button prepareBtn;
    public Button playBtn;
    public Button pauseBtn;
    public Button stopBtn;
    public Button clearBtn;

    public Text scrollText;
    public GameObject scrollPanel;

    public Text progressText;
    public Slider progressSlider;

    public Text volumeText;
    public Slider volumeSlider;


    private VideoPlayerHelper mVideoPlayer = null;
    private VideoPlayerHelper.MediaState mCurrentState = VideoPlayerHelper.MediaState.NOT_READY;

    private IntPtr texPtr = IntPtr.Zero;
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
        clearBtn.onClick.AddListener(ClearLog);

        progressSlider.minValue = 0.0f;
        progressSlider.value = 0.0f;
        progressSlider.GetComponent<SliderDrag>().EndDrag = progressSliderValueDidChange;


        volumeSlider.minValue = 0.0f;
        volumeSlider.maxValue = 1.0f;
        volumeSlider.value = 1.0f;
        volumeSlider.GetComponent<SliderDrag>().EndDrag = volumeSliderValueDidChange;


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

    void pushMsg(string msg)
    {
        StartCoroutine(coroutineAddMsg(msg));
    }

    IEnumerator coroutineAddMsg(string msg)
    {
        scrollText.text += msg + "\n";
        yield return new WaitForEndOfFrame();
        scrollPanel.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;

    }

    #region haha

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
            pushMsg("Could not initialize video player");
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

    void ClearLog()
    {
        scrollText.text = "";
    }

    void progressSliderValueDidChange()
    {
        mVideoPlayer.SeekTo((int)progressSlider.value);  
    }

    void volumeSliderValueDidChange()
    {
        volumeText.text = "音量：" + volumeSlider.value.ToString("0.##");
        mVideoPlayer.SetVolume(volumeSlider.value);

    }

    #endregion

    private void OnRenderObject()
    {

      
        if (mAppPaused) return;
        if (!mIsInited) return;
        if (!mIsPrepared)
        {
            pushMsg("!mIsPrepared....");

            VideoPlayerHelper.MediaState state = mVideoPlayer.GetStatus();

            if (state == VideoPlayerHelper.MediaState.ERROR)
            {
                pushMsg("Could not load video ");
                HandleStateChange(VideoPlayerHelper.MediaState.ERROR);
                this.enabled = false;
            }

            else if (state < VideoPlayerHelper.MediaState.NOT_READY)
            {
                // Video player is ready
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
                pushMsg("Video is prepared, ready for playback");
            }
           
        }
        else
        {

            VideoPlayerHelper.MediaState state = mVideoPlayer.UpdateVideoData();

            if (state == VideoPlayerHelper.MediaState.PLAYING)
            {
                GL.InvalidateState();
                // Update Video Info
                UpdateVideoInfo();

            }

            if (state != mCurrentState)
            {
                HandleStateChange(state);
                mCurrentState = state;
            }
        }


    }


    void UpdateVideoInfo() {
        progressSlider.maxValue = (int)mVideoPlayer.GetLength();
        progressSlider.value = (int)mVideoPlayer.GetCurrentPosition();
        progressText.text = "进度：" + progressSlider.value + " / " + progressSlider.maxValue;
        string msg = "缓冲百分比 ==> " + mVideoPlayer.GetCurrentBufferingPercentage();
        pushMsg(msg);

    }

    void OnDestroy()
    {
        // Deinit the video
        mVideoPlayer.Deinit();
    }

    private void HandleStateChange(VideoPlayerHelper.MediaState newState)
    {
    
        switch (newState)
        {
            case VideoPlayerHelper.MediaState.PLAYING:
                pushMsg("PLAYING");
                break;
            case VideoPlayerHelper.MediaState.READY:
                pushMsg("READY");
                break;

            case VideoPlayerHelper.MediaState.REACHED_END:
                pushMsg("REACHED_END");
                break;

            case VideoPlayerHelper.MediaState.PAUSED:
                pushMsg("PAUSED");
                break;

            case VideoPlayerHelper.MediaState.STOPPED:
                pushMsg("STOPPED");
                break;

            case VideoPlayerHelper.MediaState.NOT_READY:
                pushMsg("NOT_READY");
                break;

            case VideoPlayerHelper.MediaState.ERROR:
                pushMsg("ERROR");
                break;

            default:
                break;
        }
    }
}
