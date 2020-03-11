#import <UIKit/UIKit.h>

@interface AlertPlugin : NSObject
+ (void)alertWithMsg:(NSString *)msg;
@end

@implementation AlertPlugin
+ (void)alertWithMsg:(NSString *)msg {
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"Alert!" message:msg preferredStyle:UIAlertControllerStyleAlert];
    [alertController addAction:[UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:nil]];
    UIViewController *rootVC = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
    [rootVC presentViewController:alertController animated:YES completion:nil];
}
@end

extern "C" {
    void ShowAlert(const char *title) {
        [AlertPlugin alertWithMsg:[NSString stringWithUTF8String:title]];
    }
}






