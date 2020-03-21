//
//  IJKUnityPlayerWrapper.h
//  IJKMediaDemo
//
//  Created by 何宗柱 on 2020/3/13.
//  Copyright © 2020 bilibili. All rights reserved.
//

#ifndef IJKUnityPlayerWrapper_h
#define IJKUnityPlayerWrapper_h

#ifdef __cplusplus
extern "C"
{
#endif

    void* videoPlayerInitIOS();
    bool videoPlayerDeinitIOS(void* playerPtr);
    bool videoPlayerLoadIOS(void* playerPtr, const char* filename, bool playOnTextureImmediately, float seekPosition);
    bool videoPlayerUnloadIOS(void* playerPtr);
    bool videoPlayerSetVideoTexturePtrIOS(void* playerPtr, void* texturePtr);
    int videoPlayerGetStatusIOS(void* playerPtr);
    int videoPlayerGetVideoWidthIOS(void* playerPtr);
    int videoPlayerGetVideoHeightIOS(void* playerPtr);
    float videoPlayerGetLengthIOS(void* playerPtr);
    bool videoPlayerPlayIOS(void* playerPtr, float seekPosition);
    bool videoPlayerPauseIOS(void* playerPtr);
    bool videoPlayerStopIOS(void* playerPtr);
    int videoPlayerUpdateVideoDataIOS(void* playerPtr);
    bool videoPlayerSeekToIOS(void* playerPtr, float position);
    float videoPlayerGetCurrentPositionIOS(void* playerPtr);
    bool videoPlayerSetVolumeIOS(void* playerPtr, float value);
    int videoPlayerGetCurrentBufferingPercentageIOS(void* playerPtr);
    uint32_t createVideoTextureIdIOS(void* playerPtr);
    uint32_t getVideoTextureIdIOS(void* playerPtr);
#ifdef __cplusplus
}
#endif


#endif /* IJKUnityPlayerWrapper_h */
