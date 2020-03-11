using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VideoPlayerHelper : MonoBehaviour
{
    // Start is called before the first frame update
    private IntPtr mVideoPlayerPtr = IntPtr.Zero;

    [DllImport("__Internal")]
    private static extern IntPtr videoPlayerInitIOS();

    [DllImport("__Internal")]
    private static extern void videoPlayerDeinitIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerLoadIOS(IntPtr videoPlayerPtr, string filename);

    [DllImport("__Internal")]
    private static extern bool videoPlayerUnloadIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetStatusIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetVideoWidthIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern int videoPlayerGetVideoHeightIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern float videoPlayerGetLengthIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerPlayIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerPauseIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerStopIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern IntPtr videoPlayerUpdateTextureIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern bool videoPlayerSeekToIOS(IntPtr videoPlayerPtr, float position);

    [DllImport("__Internal")]
    private static extern float videoPlayerGetCurrentPositionIOS(IntPtr videoPlayerPtr);

    [DllImport("__Internal")]
    private static extern void videoPlayerSetVolumeIOS(IntPtr videoPlayerPtr, float value);


    public bool videoPlayerInit()
    {
        mVideoPlayerPtr = videoPlayerInitIOS();
        return mVideoPlayerPtr != IntPtr.Zero;
    }

    public void videoPlayerDeinit()
    {
        videoPlayerDeinitIOS(mVideoPlayerPtr);
        mVideoPlayerPtr = IntPtr.Zero;
    }

    public bool videoPlayerLoad(string filename)
    {
        return videoPlayerLoadIOS(mVideoPlayerPtr, filename);
    }

    public bool videoPlayerUnload()
    {
        return videoPlayerUnloadIOS(mVideoPlayerPtr);
    }

    public int videoPlayerGetStatus()
    {
        return videoPlayerGetStatusIOS(mVideoPlayerPtr);
    }

    public int videoPlayerGetVideoWidth()
    {
        return videoPlayerGetVideoWidthIOS(mVideoPlayerPtr);
    }

    public int videoPlayerGetVideoHeight()
    {
        return videoPlayerGetVideoHeightIOS(mVideoPlayerPtr);
    }

    public float videoPlayerGetLength()
    {
        return videoPlayerGetLengthIOS(mVideoPlayerPtr);
    }

    public bool videoPlayerPlay()
    {
        return videoPlayerPlayIOS(mVideoPlayerPtr);
    }

    public bool videoPlayerPause()
    {
        return videoPlayerPauseIOS(mVideoPlayerPtr);
    }

    public bool videoPlayerStop()
    {
        return videoPlayerStopIOS(mVideoPlayerPtr);
    }

    public IntPtr videoPlayerUpdateTextureIOS()
    {
        return videoPlayerUpdateTextureIOS(mVideoPlayerPtr);
    }
    public bool videoPlayerSeekTo(float position)
    {
        return videoPlayerSeekToIOS(mVideoPlayerPtr, position);
    }

    public float videoPlayerGetCurrentPosition()
    {
        return videoPlayerGetCurrentPositionIOS(mVideoPlayerPtr);
    }

    public void videoPlayerSetVolume(float value)
    {
        videoPlayerSetVolumeIOS(mVideoPlayerPtr, value);
    }
}
