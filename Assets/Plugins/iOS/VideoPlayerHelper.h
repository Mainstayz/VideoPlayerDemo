//
//  VideoPlayerHelper.h
//  IJKMediaDemo
//
//  Created by 何宗柱 on 2020/3/13.
//  Copyright © 2020 bilibili. All rights reserved.
//

#import <Foundation/Foundation.h>

// Media states
typedef enum tagMEDIA_STATE {
    REACHED_END         = 0,
    PAUSED              = 1,
    STOPPED             = 2,
    PLAYING             = 3,
    READY               = 4,
    NOT_READY           = 5,
    ERROR               = 6,
} MEDIA_STATE;

typedef enum tagMEDIA_FMT {
    MEDIA_FMT_ES2 = 0,
    MEDIA_FMT_I420,
    MEDIA_FMT_YU12,
    MEDIA_FMT_RV16,
    MEDIA_FMT_RV24,
    MEDIA_FMT_RV32,
    MEDIA_FMT_BGRA,
    MEDIA_FMT_RGBA,
}MEDIA_FMT;


// Used to specify that playback should start from the current position when
// calling the load and play methods
static const float VIDEO_PLAYBACK_CURRENT_POSITION = -1.0f;

@interface VideoPlayerHelper : NSObject
- (instancetype)init;
- (void)deinit;
- (void)setMediaFormat:(MEDIA_FMT)format;
- (void)tryToHardDecode:(BOOL)hardDecode;
- (BOOL)load:(NSString *)url playImmediately:(BOOL)playOnTextureImmediately fromPosition:(float)seekPosition;
- (BOOL)unload;
- (MEDIA_STATE)getStatus;
- (int)getVideoHeight;
- (int)getVideoWidth;
- (float)getLength;
- (BOOL)play:(float)seekPosition;
- (BOOL)pause;
- (BOOL)stop;
- (MEDIA_STATE)updateVideoData;
- (BOOL)seekTo:(float)position;
- (float)getCurrentPosition;
- (float)getCurrentBufferingPercentage;
- (BOOL)setVolume:(float)volume;
- (BOOL)setVideoTexturePtr:(void*)texturePtr;
- (GLuint)videoTexturePtr;
- (GLuint)createVideoTextureGL;
@end

