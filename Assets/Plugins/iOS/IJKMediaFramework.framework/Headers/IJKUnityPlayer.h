//
//  IJKUnityPlayer.h
//  IJKMediaPlayer
//
//  Created by pillar on 2020/2/5.
//  Copyright © 2020 bilibili. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "IJKMediaPlayback.h"
#import "IJKSDLGLViewProtocol.h"

typedef NS_ENUM (NSInteger, IJKUPOverlayFormat) {
    IJKUPOverlayFormatES2 = 0,
    IJKUPOverlayFormatI420,
    IJKUPOverlayFormatYU12,
    IJKUPOverlayFormatRV16,
    IJKUPOverlayFormatRV24,
    IJKUPOverlayFormatRV32,
    IJKUPOverlayFormatBGRA,
    IJKUPOverlayFormatRGBA,
};
typedef struct SDL_VoutOverlay SDL_VoutOverlay;

NS_ASSUME_NONNULL_BEGIN

@class IJKUnityPlayer;

@interface IJKUnityPlayer : NSObject

/// 播放状态
@property(nonatomic, assign, readonly) IJKMPMoviePlaybackState playbackState;

/// 加载状态
@property(nonatomic, assign, readonly) IJKMPMovieLoadState loadState;

/// 播放层，可添加到UIView的layer中
@property(nonatomic, strong, readonly) CAEAGLLayer *layer;

/// 指定格式
@property(nonatomic, assign) IJKUPOverlayFormat overlayFormat;

/// 准备完成之后自动播放
@property(nonatomic, assign) BOOL shouldAutoPlay;

/// 播放地址
@property(nonatomic, copy) NSString *url;

/// 显示模式，以下为有效选项
/// UIViewContentModeScaleToFill
/// UIViewContentModeScaleAspectFit
/// UIViewContentModeScaleAspectFill
@property(nonatomic, assign) UIViewContentMode contentMode;

/// 播放速度
@property(nonatomic, assign) CGFloat playbackRate;

/// 播放音量
/// [0,1]
@property(nonatomic, assign) CGFloat playbackVolume;

/// 当前缓冲postion
@property(nonatomic, assign) NSTimeInterval bufferingPosition;

/// 缓冲百分比
@property(nonatomic, assign) NSTimeInterval bufferingProgress;

@property(nonatomic, assign, readonly) CGFloat videoWidth;

@property(nonatomic, assign, readonly) CGFloat videoHeight;

/// 关闭视屏输出，在播放前调用有效
@property(nonatomic, assign) BOOL disableVideo;

/// 关闭音频输出，在播放前调用有效
@property(nonatomic, assign) BOOL disableAudio;

/// 循环次数
@property(nonatomic, assign) int loop;

/// 是否正在播放
@property(nonatomic, assign, readonly) BOOL isPlaying;

/// 是否打开硬解码，当播放4k高清视频时，可打开，提高GPU的利用率
@property(nonatomic, assign, getter = isHardDecode) BOOL hardDecode;

/// 设置最大帧数范围。默认31（-1 , 121）
@property(nonatomic, assign) int64_t maxFps;

/// 设置播放前的最大探测时间，单位 微妙
/// 可以看到 analyzeduration 参数不设置时，即 analyzeduration 默认等于 0 时，可以看到默认的分析时长为 5 秒:
/// 而如果是 flv 文件的时候，默认为 90 秒：
/// 如果是 mpeg 和 mpegts 文件的时候，默认 7 秒:
/// 为了减少首屏等待时间可减少改值
/// @see https://www.jianshu.com/p/37d705aa0e01
@property(nonatomic, assign) int64_t analyzeMaxDuration;

/// 播放前的探测文件，单位字节，默认 1024 * 10 = 10 kb
/// 效果同上
@property(nonatomic, assign) int64_t probesize;

///指定帧不做环路滤波，主要是用于画面去块
/// -16  啥事也不做
/// 0     丢弃无用的帧，比如0大小的帧
/// 8     丢弃所有非引用
/// 16     丢弃所有双向帧
/// 32     丢弃除关键帧之外的所有帧
/// 48     所有帧都不做环路滤波
/// 简而言之：0  画面质量高，解码开销大，48，画面质量差点，解码开销小
@property(nonatomic, assign) int64_t skipLoopFilter;

///跳帧处理,放CPU处理较慢时，进行跳帧处理，保证播放流程，画面和声音同步
@property(nonatomic, assign) int64_t allowFrameDropCount;

/// 不限制输入缓存大小，常用语直播流
@property(nonatomic, assign) BOOL infbuf;

/// 预缓冲，一般直播项目会开启，达到秒开的效果
@property(nonatomic, assign, getter = isBufferCache) BOOL bufferCache;

/// 超时
@property(nonatomic, assign) int64_t timeout;

@property(nonatomic, weak) id<IJKCVPBViewProtocol> cvPBView;

- (instancetype)init;

/// 请设置 cvPBView 的值，自解析 CVPixelBufferRef
/// WARNNING: 模拟器无效
- (instancetype)initWithFboWithContext:(nullable EAGLContext *)context;

/// 设置Logd级别
/// @param level int
/// IJK_LOG_UNKNOWN     0
/// IJK_LOG_DEFAULT     1
/// IJK_LOG_VERBOSE     2
/// IJK_LOG_DEBUG       3
/// IJK_LOG_INFO        4
/// IJK_LOG_WARN        5
/// IJK_LOG_ERROR       6
/// IJK_LOG_FATAL       7
/// IJK_LOG_SILENT      8
+ (void)setLogLevel:(int)level;

/// 准备数据，如果shouldAutoPlay为YES,则自动播放
- (int)prepareAsync;

/// 开始播放
- (int)start;

/// 停止播放
- (int)stop;

/// 暂停
- (int)pause;

/// 重置
- (int)reset;

/// 关闭播放器
- (void)shutdown;

/// 关闭渲染, 可在播放过程中关闭图像渲染
/// @param disable  YES 为关闭视频渲染
- (void)displayDisable:(BOOL)disable;

/// 下载速度，单位 byte，
/// 请自行添加定时器获取实时速度
- (int64_t)tcpSpeed;

/// 已经格式化的下载速度，如 1 MB/s。
/// 请自行添加定时器获取实时速度
- (NSString *)speedFormat;

/// 获取当前位置，单位：秒
- (NSTimeInterval)getCurrentPosition;

/// 获取总长度，单位：秒
- (NSTimeInterval)getDuration;

/// 跳转到
/// @param sec 秒
- (int)seekTo:(NSTimeInterval)sec;

/// 用于提供
/// @param overlay ijkplayer 的渲染输出
- (void)display:(nullable SDL_VoutOverlay *)overlay;

/// 视频解码器描述
- (NSString *)vdec;

/// 获取实时帧数
- (NSString *)realFps;

/// 获取播放器float属性
/// @param property 属性标示
/// @param value 不存在时返回值
/// @see ff_ffmsg.h
- (float)getFloatProperty:(int)property defalut:(float)value;

/// 获取播放器long属性
/// @param property 属性标示
/// @param value 不存在时返回值
/// @see ff_ffmsg.h
- (int64_t)getLongProperty:(int)property default:(int64_t)value;

/// 设置播放器选项
/// @param value 值
/// @param key 选项
- (void)setPlayerOptionValue:(NSString *)value forKey:(NSString *)key;

/// 设置播放器选项
/// @param value 值
/// @param key 选项
- (void)setPlayerOptionIntValue:(int64_t)value forKey:(NSString *)key;

/// 设置format选项
/// @param value 值
/// @param key 选项
/// https://github.com/FFmpeg/FFmpeg/blob/release/3.4/libavformat/options_table.h
- (void)setFormatOptionValue:(NSString *)value forKey:(NSString *)key;

/// 设置format选项
/// @param value 值
/// @param key 选项
/// https://github.com/FFmpeg/FFmpeg/blob/release/3.4/libavformat/options_table.h
- (void)setFormatOptionIntValue:(int64_t)value forKey:(NSString *)key;

/// 设置编码选项
/// @param value 值
/// @param key 选项
- (void)setCodecOptionValue:(NSString *)value forKey:(NSString *)key;

/// 设置编码选项
/// @param value 值
/// @param key 选项
- (void)setCodecOptionIntValue:(int64_t)value forKey:(NSString *)key;

/// 设置缩放选项
/// @param value 值
/// @param key 选项
- (void)setSwsOptionValue:(NSString *)value forKey:(NSString *)key;

/// 设置缩放选项
/// @param value 值
/// @param key 选项
- (void)setSwsOptionIntValue:(int64_t)value forKey:(NSString *)key;

@end

NS_ASSUME_NONNULL_END
