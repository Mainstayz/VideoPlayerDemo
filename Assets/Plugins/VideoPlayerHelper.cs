using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VideoPlayerHelper : MonoBehaviour
{
    #region NESTED
    public enum MediaState
    {
        REACHED_END,
        PAUSED,
        STOPPED,
        PLAYING,
        READY,
        NOT_READY,
        ERROR,
    }
    #endregion

    private IntPtr mVideoPlayerPtr = IntPtr.Zero;

    [DllImport("__Internal")]
    private static extern IntPtr videoPlayerInitIOS();

    [DllImport("__Internal")]
    private static extern bool videoPlayerDeinitIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerLoadIOS(IntPtr videoPlayerPtr, string filename, bool playOnTextureImmediately, float seekPosition);

    [DllImport("__Internal")]
    private static extern bool videoPlayerUnloadIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerSetVideoTexturePtrIOS(IntPtr videoPlayerPtr, IntPtr texturePtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetStatusIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetVideoWidthIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetVideoHeightIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern float videoPlayerGetLengthIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerPlayIOS(IntPtr videoPlayerPtr, float seekPosition);

    [DllImport("__Internal")]
    private static extern bool videoPlayerPauseIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerStopIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerUpdateVideoDataIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerSeekToIOS(IntPtr videoPlayerPtr, float position);

    [DllImport("__Internal")]
    private static extern float videoPlayerGetCurrentPositionIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerSetVolumeIOS(IntPtr videoPlayerPtr, float value);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetCurrentBufferingPercentageIOS(IntPtr videoPlayerPtr);

    
    [DllImport("__Internal")]
    private static extern IntPtr createVideoTextureIdIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern IntPtr getVideoTextureIdIOS(IntPtr videoPlayerPtr);


    private bool videoPlayerInit()
    {
        mVideoPlayerPtr = videoPlayerInitIOS();
        return mVideoPlayerPtr != IntPtr.Zero;
    }

    private bool videoPlayerDeinit()
    {
        bool result = videoPlayerDeinitIOS(mVideoPlayerPtr);
        mVideoPlayerPtr = IntPtr.Zero;
        return result;
    }

    private bool videoPlayerLoad(string filename, bool playOnTextureImmediately, float seekPosition)
    {
        return videoPlayerLoadIOS(mVideoPlayerPtr, filename, playOnTextureImmediately, seekPosition);
    }

    private bool videoPlayerUnload()
    {
        return videoPlayerUnloadIOS(mVideoPlayerPtr);
    }

    private bool videoPlayerSetVideoTexturePtr(IntPtr texturePtr)
    {
        return videoPlayerSetVideoTexturePtrIOS(mVideoPlayerPtr, texturePtr);
    }

    private int videoPlayerGetStatus()
    {
        return videoPlayerGetStatusIOS(mVideoPlayerPtr);
    }

    private int videoPlayerGetVideoWidth()
    {
        return videoPlayerGetVideoWidthIOS(mVideoPlayerPtr);
    }

    private int videoPlayerGetVideoHeight()
    {
        return videoPlayerGetVideoHeightIOS(mVideoPlayerPtr);
    }

    private float videoPlayerGetLength()
    {
        return videoPlayerGetLengthIOS(mVideoPlayerPtr);
    }

    private bool videoPlayerPlay(float seekPosition)
    {
        return videoPlayerPlayIOS(mVideoPlayerPtr, seekPosition);
    }

    private bool videoPlayerPause()
    {
        return videoPlayerPauseIOS(mVideoPlayerPtr);
    }

    private bool videoPlayerStop()
    {
        return videoPlayerStopIOS(mVideoPlayerPtr);
    }

    private int videoPlayerUpdateVideoData()
    {
        return videoPlayerUpdateVideoDataIOS(mVideoPlayerPtr);
    }

    private bool videoPlayerSeekTo(float position)
    {
        return videoPlayerSeekToIOS(mVideoPlayerPtr, position);
    }

    private float videoPlayerGetCurrentPosition()
    {
        return videoPlayerGetCurrentPositionIOS(mVideoPlayerPtr);
    }

    private bool videoPlayerSetVolume(float value)
    {
        return videoPlayerSetVolumeIOS(mVideoPlayerPtr, value);
    }

    private int videoPlayerGetCurrentBufferingPercentage()
    {
        return videoPlayerGetCurrentBufferingPercentageIOS(mVideoPlayerPtr);
    }

    private IntPtr createVideoTextureId()
    {
        return createVideoTextureIdIOS(mVideoPlayerPtr);
    }

    private IntPtr getVideoTextureId()
    {
        return getVideoTextureIdIOS(mVideoPlayerPtr);
    }

    #region PRIVATE_METHODS

    private string mFilename = null;

    #endregion

    #region PUBLIC_METHODS

    public void SetFilename(string filename)
    {

        mFilename = filename;
    }

    public bool Init()
    {
        return videoPlayerInit();
    }

    public bool Deinit()
    {
        return videoPlayerDeinit();
    }

    public bool Load(string filename, bool playOnTextureImmediately, float seekPosition)
    {
        SetFilename(filename);
        return videoPlayerLoad(mFilename, playOnTextureImmediately, seekPosition);
    }

    public bool Unload()
    {
        return videoPlayerUnload();
    }

    public bool SetVideoTexturePtr(IntPtr texturePtr)
    {
        return videoPlayerSetVideoTexturePtr(texturePtr);
    }

    public MediaState GetStatus()
    {
        return (MediaState)videoPlayerGetStatus();
    }

    public int GetVideoWidth()
    {
        return videoPlayerGetVideoWidth();
    }

    public int GetVideoHeight()
    {
        return videoPlayerGetVideoHeight();
    }

    public float GetLength()
    {
        return videoPlayerGetLength();
    }

    public bool Play(float seekPosition)
    {
        return videoPlayerPlay(seekPosition);
    }

    public bool Pause()
    {
        return videoPlayerPause();
    }

    public bool Stop()
    {
        return videoPlayerStop();
    }

    public MediaState UpdateVideoData()
    {
        return (MediaState)videoPlayerUpdateVideoData();
    }

    public bool SeekTo(float position)
    {
        return videoPlayerSeekTo(position);
    }

    public float GetCurrentPosition()
    {
        return videoPlayerGetCurrentPosition();
    }

    public bool SetVolume(float value)
    {
        return videoPlayerSetVolume(value);
    }

    public int GetCurrentBufferingPercentage()
    {
        return videoPlayerGetCurrentBufferingPercentage();
    }

    public IntPtr CreatVideoTextureId()
    {
        return createVideoTextureId();
    }

    public IntPtr GetVideoTextureId()
    {
        return getVideoTextureId();
    }
    #endregion
}
