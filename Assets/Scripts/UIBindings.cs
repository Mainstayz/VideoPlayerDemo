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
    private VideoPlayerHelper videoPlayer;
    private IntPtr textureId;
    private Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        // 添加监听
        inputField.onEndEdit.AddListener(TextFieldOnEndEdit);
        prepareBtn.onClick.AddListener(Prepare);
        playBtn.onClick.AddListener(Play);
        pauseBtn.onClick.AddListener(Pause);
        stopBtn.onClick.AddListener(Stop);

        displayLayer.gameObject.transform.rotation = Quaternion.Euler(180, 0, 0);

        // 初始化播放器
        videoPlayer = gameObject.AddComponent<VideoPlayerHelper>();
        videoPlayer.videoPlayerInit();
    
    }

    void TextFieldOnEndEdit(string arg0)
    {
        
    }

    void Prepare()
    {
        string url = inputField.text;
        videoPlayer.videoPlayerLoad(url);
    }

    void Play()
    {
        videoPlayer.videoPlayerPlay();
    }

    void Pause()
    {
        videoPlayer.videoPlayerPause();
    }

    void Stop()
    {
        videoPlayer.videoPlayerStop();
    }


    void UpdateTexture()
    {
        if (texture == null)
        {
            Debug.Log("create external texture");
            texture = new Texture2D(0, 0, TextureFormat.RGB565, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
        }

        texture.UpdateExternalTexture((IntPtr)textureId);

        displayLayer.material.mainTexture = texture;


        //displayLayer.texture = texture;

        //if (GetComponent<RawImage>() != null)
        //{
        //    GetComponent<RawImage>().texture = texture;
        //    GetComponent<RawImage>().color = Color.white;
        //}
        //else
        //{
        //    GetComponent<Renderer>().material.mainTexture = texture;
        //}
    }


    // Update is called once per frame
    void Update()
    {
        textureId = videoPlayer.videoPlayerUpdateTextureIOS();
        if ((int)textureId == 0)
        {
            return;
        } else
        {
            UpdateTexture();
        }
    }
}
