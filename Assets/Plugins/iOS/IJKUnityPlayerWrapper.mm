//
//  IJKUnityPlayerWrapper.m
//  IJKMediaDemo
//
//  Created by pillar on 2020/3/11.
//  Copyright © 2020 bilibili. All rights reserved.
//

#import <IJKMediaFramework/IJKMediaFramework.h>
#include <OpenGLES/ES2/glext.h>

@interface IJKUnityPlayerWrapper : NSObject <IJKCVPBViewProtocol>
@property (nonatomic, strong) IJKUnityPlayer *player;
@property (nonatomic, assign) CVPixelBufferRef bufferRef;
@property (nonatomic, strong) NSLock *latestSampleBufferLock;
@property (nonatomic, assign) GLuint texId;
@property (nonatomic, strong) EAGLContext *ctx;
@end

@implementation IJKUnityPlayerWrapper
- (instancetype)init {
    self = [super init];
    if (self) {
        _player = [[IJKUnityPlayer alloc] initWithFbo];
        _player.cvPBView = self;
        _player.overlayFormat = IJKUPOverlayFormatES2;
        _latestSampleBufferLock = [[NSLock alloc] init];
        _bufferRef = NULL;
        _ctx = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2];

    }
    return self;
}

- (void)display_pixelbuffer:(CVPixelBufferRef)pixelbuffer {
    _bufferRef = pixelbuffer;
}

- (GLuint)curTexture {
    
    if (_bufferRef == NULL) {
        return 0;
    }
    
    EAGLContext *prevContext = [EAGLContext currentContext];
    [EAGLContext setCurrentContext:_ctx];

    
    GLuint textureID = 0;
    CVPixelBufferLockBaseAddress(_bufferRef, kCVPixelBufferLock_ReadOnly);
    
    GLsizei texWidth    = (GLsizei)CVPixelBufferGetWidth(_bufferRef);
    GLsizei texHeight   = (GLsizei)CVPixelBufferGetHeight(_bufferRef);
    GLvoid *baseAddress = CVPixelBufferGetBaseAddress(_bufferRef);
    
    glGenTextures(1, &textureID);
    
    // bind
    glBindTexture(GL_TEXTURE_2D, textureID);
    
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, texWidth, texHeight, 0, GL_BGRA, GL_UNSIGNED_BYTE, baseAddress);
    GLenum glError = glGetError();
       /*
        #define GL_NO_ERROR                                      0
        #define GL_INVALID_ENUM                                  0x0500
        #define GL_INVALID_VALUE                                 0x0501
        #define GL_INVALID_OPERATION                             0x0502
        #define GL_OUT_OF_MEMORY                                 0x0505
        */
       
       if (GL_NO_ERROR != glError) {
           NSLog(@"glError %x\n", glError);
       }
    
    if (_texId != 0) {
      glDeleteTextures(1, &_texId);
    }
    _texId = textureID;
    glBindTexture(GL_TEXTURE_2D, 0);
    CVPixelBufferUnlockBaseAddress(_bufferRef, kCVPixelBufferLock_ReadOnly);
    [EAGLContext setCurrentContext:prevContext];
    return textureID;
}
@end

extern "C" {



    void *videoPlayerInitIOS() {
        IJKUnityPlayerWrapper *wrapper = [IJKUnityPlayerWrapper new];
        return (__bridge_retained void *)wrapper;
    }

    void videoPlayerDeinitIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        [wrapper.player shutdown];
        playerPtr = NULL;
    }

    bool videoPlayerLoadIOS(void *playerPtr, const char *filename) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        wrapper.player.url = [NSString stringWithUTF8String:filename];
        wrapper.player.url = @"http://192.168.1.6:8887/1.ts";
        return [wrapper.player prepareAsync] == 0 ? 1 : 0;
    }

    bool videoPlayerUnloadIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return [wrapper.player reset] == 0 ? 1 : 0;
    }

    int videoPlayerGetStatusIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return (int)wrapper.player.playbackState;
    }

    int videoPlayerGetVideoWidthIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return (int)wrapper.player.videoWidth;
    }

    int videoPlayerGetVideoHeightIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return (int)wrapper.player.videoHeight;
    }

    float videoPlayerGetLengthIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return (float)wrapper.player.getDuration;
    }

    bool videoPlayerPlayIOS(void *playerPtr, float seekPosition) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        if (!wrapper.player.isPlaying) {
            [wrapper.player start];
        }
        return [wrapper.player seekTo:seekPosition];
    }

    bool videoPlayerPauseIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return [wrapper.player pause] == 0 ? 1 : 0;
    }


    bool videoPlayerStopIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return [wrapper.player stop] == 0 ? 1 : 0;
    }


    long videoPlayerUpdateTextureIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return [wrapper curTexture];
    }

    bool videoPlayerSeekToIOS(void *playerPtr, float position) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return [wrapper.player seekTo:position];
    }

    float videoPlayerGetCurrentPositionIOS(void *playerPtr) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        return [wrapper.player getCurrentPosition];
    }

    void videoPlayerSetVolumeIOS(void *playerPtr, float value) {
        IJKUnityPlayerWrapper *wrapper = (__bridge IJKUnityPlayerWrapper *)(playerPtr);
        [wrapper.player setPlaybackVolume:value];
    }

}
