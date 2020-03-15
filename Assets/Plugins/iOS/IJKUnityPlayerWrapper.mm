//
//  IJKUnityPlayerWrapper.m
//  IJKMediaDemo
//
//  Created by pillar on 2020/3/11.
//  Copyright © 2020 bilibili. All rights reserved.
//
#import "IJKUnityPlayerWrapper.h"
#import "VideoPlayerHelper.h"
void* videoPlayerInitIOS() {
    VideoPlayerHelper *helper = [[VideoPlayerHelper alloc] init];
    [helper setMediaFormat:MEDIA_FMT_ES2];
    [helper tryToHardDecode:YES];
    return (__bridge_retained void *)helper;
}
bool videoPlayerDeinitIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    [helper deinit];
    free(playerPtr);
    return YES;
}
bool videoPlayerLoadIOS(void* playerPtr, const char* filename, bool playOnTextureImmediately, float seekPosition) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper load:[NSString stringWithUTF8String:filename] playImmediately:playOnTextureImmediately fromPosition:seekPosition];
}
bool videoPlayerUnloadIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper unload];
}
bool videoPlayerSetVideoTexturePtrIOS(void* playerPtr, void* texturePtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper setVideoTexturePtr:texturePtr];
}
int videoPlayerGetStatusIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper getStatus];
}
int videoPlayerGetVideoWidthIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper getVideoWidth];
}
int videoPlayerGetVideoHeightIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper getVideoHeight];
}
float videoPlayerGetLengthIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper getLength];
}
bool videoPlayerPlayIOS(void* playerPtr, float seekPosition) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper play:seekPosition];
}
bool videoPlayerPauseIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper pause];
}
bool videoPlayerStopIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper stop];
}
int videoPlayerUpdateVideoDataIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper updateVideoData];
}
bool videoPlayerSeekToIOS(void* playerPtr, float position) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper seekTo:position];
}
float videoPlayerGetCurrentPositionIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper getCurrentPosition];
}
bool videoPlayerSetVolumeIOS(void* playerPtr, float value) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper setVolume:value];
}
int videoPlayerGetCurrentBufferingPercentageIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper getCurrentBufferingPercentage];
}

uint32_t createVideoTextureIdIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper createVideoTextureGL];
}

uint32_t getVideoTextureIdIOS(void* playerPtr) {
    VideoPlayerHelper *helper = (__bridge VideoPlayerHelper*)playerPtr;
    return [helper videoTexturePtr];
}

