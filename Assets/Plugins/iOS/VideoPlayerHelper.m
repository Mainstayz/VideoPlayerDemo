//
//  VideoPlayerHelper.m
//  IJKMediaDemo
//
//  Created by 何宗柱 on 2020/3/13.
//  Copyright © 2020 bilibili. All rights reserved.
//

#import "VideoPlayerHelper.h"
#include <OpenGLES/ES2/glext.h>
#import <IJKMediaFramework/IJKMediaFramework.h>

// The number of bytes per texel (when using kCVPixelFormatType_32BGRA)
static const int BYTES_PER_TEXEL = 4;

@interface VideoPlayerHelper () <IJKCVPBViewProtocol>

@property(nonatomic, assign) GLuint videoTextureIdGL;
@property(nonatomic, assign) BOOL seekRequested;

@property(nonatomic, assign) CVPixelBufferRef latestPixelBuffer;

@property(nonatomic, assign) CGSize videoSize;

@property(nonatomic, strong) IJKUnityPlayer *nativePlayer;
@property(nonatomic, assign) MEDIA_STATE state;
@property(nonatomic, assign) float seekPosition;
@property(nonatomic, assign) BOOL needPlay;
@end

@implementation VideoPlayerHelper

- (instancetype)init {
    self = [super init];
    if (self) {
        _state = NOT_READY;
        _seekRequested = NO;
        _seekPosition = VIDEO_PLAYBACK_CURRENT_POSITION;
        _needPlay = NO;
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(playerPlaybackDidFinish:) name:IJKMoviePlayerPlaybackDidFinishNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(playbackStateDidChange:) name:IJKMoviePlayerPlaybackStateDidChangeNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(playerIsPreparedToPlay:) name:IJKMediaPlaybackIsPreparedToPlayDidChangeNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(playerLoadStateDidChange:) name:IJKMoviePlayerLoadStateDidChangeNotification object:nil];
    }
    return self;
}

- (void)deinit {
    if (!_nativePlayer) {
        return;
    }
    if ([_nativePlayer isPlaying]) {
        [_nativePlayer stop];
    }
    [_nativePlayer shutdown];
    _nativePlayer = nil;
}

- (void)setMediaFormat:(MEDIA_FMT)format {
    self.nativePlayer.overlayFormat = (IJKUPOverlayFormat)format;
}
- (void)tryToHardDecode:(BOOL)hardDecode {
    self.nativePlayer.hardDecode = hardDecode;
}
- (void)resetData {

    _state = NOT_READY;
    _videoTextureIdGL = 0;
    _seekRequested = NO;
    _seekPosition = VIDEO_PLAYBACK_CURRENT_POSITION;
    _needPlay = NO;
}

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    [self deinit];
}

#pragma mark - public

- (BOOL)load:(NSString *)url playImmediately:(BOOL)playOnTextureImmediately fromPosition:(float)seekPosition {
    if (_state != NOT_READY && _state != ERROR) {
        NSLog(@"Media already loaded.  Unload current media first.");
        return NO;
    }

    self.nativePlayer.url = url;
    self.nativePlayer.shouldAutoPlay = playOnTextureImmediately;
    self.seekPosition = seekPosition;
    return [self.nativePlayer prepareAsync] == 0;
}

- (BOOL)unload {
    if ([self.nativePlayer isPlaying]) {
        [self.nativePlayer stop];
    }
    [self resetData];
    return [self.nativePlayer reset];
}

- (MEDIA_STATE)getStatus {
    return _state;
}

- (int)getVideoWidth {
    int ret = -1;
    if (_state < NOT_READY) {
        ret = _videoSize.width;
    } else {
        NSLog(@"Video width not available in current state");
    }
    return ret;
}

- (int)getVideoHeight {
    int ret = -1;
    if (_state < NOT_READY) {
        ret = _videoSize.height;
    } else {
        NSLog(@"Video height not available in current state");
    }
    return ret;
}

- (float)getLength {
    float ret = -1;
    if (_state < NOT_READY) {
        ret = self.nativePlayer.getDuration;
    } else {
        NSLog(@"Video length not available in current state");
    }
    return ret;
}

- (float)getCurrentPosition {
    float ret = -1;
    if (_state < NOT_READY) {
        ret = self.nativePlayer.getCurrentPosition;
    } else {
        NSLog(@"Get current position not available in current state");
    }
    return ret;
}

- (BOOL)play:(float)seekPosition {

    if (seekPosition == VIDEO_PLAYBACK_CURRENT_POSITION) {
        if (self.nativePlayer.isPlaying) {
            return YES;
        }
        return [self.nativePlayer start] == 0;
    }

    if (self.nativePlayer.isPlaying) {
        [self.nativePlayer pause];
        self.needPlay = YES;
    }

    return [self seekTo:seekPosition] == 0;
}

- (BOOL)pause {
    if (self.nativePlayer.isPlaying) {
        return [self.nativePlayer pause] == 0;
    }
    return YES;
}

- (BOOL)stop {
    if (self.nativePlayer.isPlaying) {
        return [self.nativePlayer stop] == 0;
    }
    return YES;
}

- (BOOL)seekTo:(float)position {
    int ret = -1;
    if (_state < NOT_READY) {
        ret = [self.nativePlayer seekTo:position];;
    } else {
        NSLog(@"Can`t seek  in current state");
    }
    return ret;
}

- (BOOL)setVolume:(float)volume {
    self.nativePlayer.playbackVolume = volume;
    return YES;
}

/// 保存外部转入的纹理
/// @param texturePtr 纹理id
- (BOOL)setVideoTexturePtr:(void *)texturePtr {
    _videoTextureIdGL = (int)((long)texturePtr);
    // 清空纹理内容
    glDeleteTextures(GL_TEXTURE_2D, &_videoTextureIdGL);
    // 设置纹理属性
    glBindTexture(GL_TEXTURE_2D, _videoTextureIdGL);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glBindTexture(GL_TEXTURE_2D, 0);
    return YES;
}
- (GLuint)videoTexturePtr; {
    return _videoTextureIdGL;
}

- (float)getCurrentBufferingPercentage {
    float ret = -1;
    if (_state < NOT_READY) {
        ret = self.nativePlayer.bufferingProgress;
    } else {
        NSLog(@"buffering percentage not available in current state");
    }
    return ret;
}
#define FourCC2Str(fourcc) (const char[]){*(((char*)&fourcc)+3), *(((char*)&fourcc)+2), *(((char*)&fourcc)+1), *(((char*)&fourcc)+0),0}


- (void)display_pixelbuffer:(CVPixelBufferRef)pixelBuffer {
    _latestPixelBuffer = pixelBuffer;
}

- (MEDIA_STATE)updateVideoData {
    if (_state == PLAYING) {
        GLvoid *pixelBufferBaseAddress = NULL;

        CVPixelBufferRef pixelBuffer;

        if (_latestPixelBuffer != NULL) {
            pixelBuffer = _latestPixelBuffer;
            CVPixelBufferLockBaseAddress(pixelBuffer, 0);
            pixelBufferBaseAddress = CVPixelBufferGetBaseAddress(pixelBuffer);
        } else {
            NSLog(@"No video sample buffer available");
        }

        if (pixelBufferBaseAddress != NULL) {

            if (_videoTextureIdGL == 0) {
                _videoTextureIdGL = [self createVideoTextureGL];
            }

            glBindTexture(GL_TEXTURE_2D, _videoTextureIdGL);

            const size_t bytesPerRow = CVPixelBufferGetBytesPerRow(pixelBuffer);

            if (bytesPerRow / BYTES_PER_TEXEL == _videoSize.width) {
                // No padding between lines of decoded video
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, _videoSize.width, _videoSize.height, 0, GL_BGRA, GL_UNSIGNED_BYTE, pixelBufferBaseAddress);
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
            } else {
                // Allocate storage for the OpenGL texture (correctly sized)
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, _videoSize.width, _videoSize.height, 0, GL_BGRA, GL_UNSIGNED_BYTE, NULL);

                // Now upload each line of texture data as a sub-image
                for (int i = 0; i < _videoSize.height; ++i) {
                    GLubyte* line = pixelBufferBaseAddress + i * bytesPerRow;
                    glTexSubImage2D(GL_TEXTURE_2D, 0, 0, i, _videoSize.width, 1, GL_BGRA, GL_UNSIGNED_BYTE, line);
                }
            }

            glBindTexture(GL_TEXTURE_2D, 0);
            CVPixelBufferUnlockBaseAddress(pixelBuffer, 0);
        }
    }
    return _state;
}



#pragma mark - notification

- (void)playerIsPreparedToPlay:(NSNotification *)notification {
    _videoSize = CGSizeMake(self.nativePlayer.videoWidth, self.nativePlayer.videoHeight);
    if (self.seekPosition >= 0) {
        [self seekTo:self.seekPosition];
    }
}

- (void)playerLoadStateDidChange:(NSNotification *)notification {
    if (self.nativePlayer.loadState & IJKMPMovieLoadStatePlaythroughOK ||
        self.nativePlayer.loadState & IJKMPMovieLoadStatePlayable) {
        _state = READY;
        if (self.needPlay) {
            [self.nativePlayer start];
            self.needPlay = NO;
        }
    }
}

- (void)playbackStateDidChange:(NSNotification *)notification {
    switch (self.nativePlayer.playbackState) {
        case IJKMPMoviePlaybackStateStopped:
            _state = STOPPED;
            break;
        case IJKMPMoviePlaybackStatePlaying:
            _state = PLAYING;
            break;
        case IJKMPMoviePlaybackStatePaused:
        case IJKMPMoviePlaybackStateInterrupted:
        case IJKMPMoviePlaybackStateSeekingForward:
        case IJKMPMoviePlaybackStateSeekingBackward:
            _state = PAUSED;
        break;
        default:
            break;
    }
}

- (void)playerPlaybackDidFinish:(NSNotification *)notification {
    NSDictionary *userInfo = notification.userInfo;
    NSInteger reason = [userInfo[IJKMoviePlayerPlaybackDidFinishReasonUserInfoKey] interval];
    switch (reason) {
        case IJKMPMovieFinishReasonPlaybackEnded:
            _state = REACHED_END;
            break;
        case IJKMPMovieFinishReasonPlaybackError:
            _state = ERROR;
            NSLog(@"playerPlaybackDidFinish Error:%@",userInfo);
            break;
        case IJKMPMovieFinishReasonUserExited:
            _state = STOPPED;
            NSLog(@"playerPlaybackDidFinish: UserExited");
            break;
        default:
            break;
    }


}

#pragma mark - lazy

- (GLuint)createVideoTextureGL {
    GLuint handle;
    glGenTextures(1, &handle);
    glBindTexture(GL_TEXTURE_2D, handle);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glBindTexture(GL_TEXTURE_2D, 0);
    return handle;
}

- (IJKUnityPlayer *)nativePlayer {
    if (!_nativePlayer) {
        _nativePlayer = [[IJKUnityPlayer alloc] initWithFboWithContext:nil];
        _nativePlayer.cvPBView = self;
    }
    return _nativePlayer;
}
@end
