#include "UnityAppController+ViewHandling.h"
#include "UnityAppController+Rendering.h"

#include "UI/OrientationSupport.h"
#include "UI/UnityView.h"
#include "UI/UnityViewControllerBase.h"
#include "Unity/DisplayManager.h"


// TEMP: ?
#include "UI/ActivityIndicator.h"
#include "UI/SplashScreen.h"
#include "UI/Keyboard.h"
#include <utility>

extern bool _skipPresent;
extern bool _unityAppReady;


@implementation UnityAppController (ViewHandling)

#if UNITY_SUPPORT_ROTATION
// special case for when we DO know the app orientation, but dont get it through normal mechanism (UIViewController orientation handling)
// how can this happen:
// 1. On startup: ios is not sending "change orientation" notifications on startup (but rather we "start" in correct one already)
// 2. When using presentation controller it can override orientation constraints, so on dismissing we need to tweak app orientation;
//      pretty much like startup situation UIViewController would have correct orientation, and app will be out-of-sync
- (void)updateAppOrientation:(UIInterfaceOrientation)orientation
{
    _curOrientation = orientation;
    [_unityView willRotateToOrientation: orientation fromOrientation: (UIInterfaceOrientation)UIInterfaceOrientationUnknown];
    [_unityView didRotate];
}

#endif

- (UnityView*)createUnityView
{
    return [[UnityView alloc] initFromMainScreen];
}

- (UIViewController*)createUnityViewControllerDefault
{
    UnityDefaultViewController* ret = [[UnityDefaultViewController alloc] init];
#if PLATFORM_TVOS
    // This enables game controller use in on-screen keyboard
    ret.controllerUserInteractionEnabled = YES;
#endif
    return ret;
}

#if UNITY_SUPPORT_ROTATION
- (UIViewController*)createUnityViewControllerForOrientation:(UIInterfaceOrientation)orient
{
    switch (orient)
    {
        case UIInterfaceOrientationPortrait:            return [[UnityPortraitOnlyViewController alloc] init];
        case UIInterfaceOrientationPortraitUpsideDown:  return [[UnityPortraitUpsideDownOnlyViewController alloc] init];
        case UIInterfaceOrientationLandscapeLeft:       return [[UnityLandscapeLeftOnlyViewController alloc] init];
        case UIInterfaceOrientationLandscapeRight:      return [[UnityLandscapeRightOnlyViewController alloc] init];

        default:                                        NSAssert(false, @"bad UIInterfaceOrientation provided");
    }
    return nil;
}

#endif

- (UIViewController*)createRootViewController
{
    UIViewController* ret = nil;
    if (!UNITY_SUPPORT_ROTATION || UnityShouldAutorotate())
    {
        if (_viewControllerForOrientation[0] == nil)
            _viewControllerForOrientation[0] = [self createUnityViewControllerDefault];
        ret = _viewControllerForOrientation[0];
    }

#if UNITY_SUPPORT_ROTATION
    if (ret == nil)
    {
        UIInterfaceOrientation orientation = ConvertToIosScreenOrientation((ScreenOrientation)UnityRequestedScreenOrientation());
        ret = [self createRootViewControllerForOrientation: orientation];
    }

    if (_curOrientation == UIInterfaceOrientationUnknown)
        [self updateAppOrientation: ConvertToIosScreenOrientation(UIViewControllerOrientation(ret))];
#endif
    return ret;
}

#if UNITY_SUPPORT_ROTATION
- (UIViewController*)createSecondaryAutorotatingViewController
{
    if (_secondaryAutorotatingViewController == nil)
        _secondaryAutorotatingViewController = [self createUnityViewControllerDefault];
    std::swap(_viewControllerForOrientation[0], _secondaryAutorotatingViewController);
    return _viewControllerForOrientation[0];
}

#endif

- (UIViewController*)topMostController
{
    UIViewController *topController = self.window.rootViewController;
    while (topController.presentedViewController)
        topController = topController.presentedViewController;
    return topController;
}

- (void)willStartWithViewController:(UIViewController*)controller
{
    _unityView.contentScaleFactor   = UnityScreenScaleFactor([UIScreen mainScreen]);
    _unityView.autoresizingMask     = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;

    _rootController.view = _rootView = _unityView;
}

- (void)willTransitionToViewController:(UIViewController*)toController fromViewController:(UIViewController*)fromController
{
}

- (UIView*)createSnapshotView
{
    // Snapshot API appeared on iOS 7, however before iOS 8 tweaking hierarchy like that on going to
    // background results in all kind of weird things when going back to foreground so we do snapshotting
    // only on iOS 8 and newer.

    // Note that on iPads with iOS 9 or later (up to iOS 10.2 at least) there's a bug in the iOS
    // compositor: any use of -[UIView snapshotViewAfterScreenUpdates] causes black screen being shown
    // temporarily when 4 finger gesture to swipe to another app in the task switcher is being performed slowly
#if UNITY_SNAPSHOT_VIEW_ON_APPLICATION_PAUSE
    return _ios80orNewer ? [_rootView snapshotViewAfterScreenUpdates: YES] : nil;
#else
    return nil;
#endif
}

- (void)createUI
{
    NSAssert(_unityView != nil, @"_unityView should be inited at this point");
    NSAssert(_window != nil, @"_window should be inited at this point");

    _rootController = [self createRootViewController];

    [self willStartWithViewController: _rootController];

    NSAssert(_rootView != nil, @"_rootView  should be inited at this point");
    NSAssert(_rootController != nil, @"_rootController should be inited at this point");

    [_window makeKeyAndVisible];
    [UIView setAnimationsEnabled: NO];

    // TODO: extract it?

    ShowSplashScreen(_window);

    NSNumber* style = [[[NSBundle mainBundle] infoDictionary] objectForKey: @"Unity_LoadingActivityIndicatorStyle"];
    ShowActivityIndicator([SplashScreen Instance], style ? [style intValue] : -1);
}

- (void)showGameUI
{
    HideActivityIndicator();
    HideSplashScreen();

    // make sure that we start up with correctly created/inited rendering surface
    // NB: recreateRenderingSurface won't go into rendering because _unityAppReady is false
#if UNITY_SUPPORT_ROTATION
    [self checkOrientationRequest];
#endif
    [_unityView recreateRenderingSurface];

    // UI hierarchy
    [_window addSubview: _rootView];
    _window.rootViewController = _rootController;
    [_window bringSubviewToFront: _rootView];

    // why we set level ready only now:
    // surface recreate will try to repaint if this var is set (poking unity to do it)
    // but this frame now is actually the first one we want to process/draw
    // so all the recreateSurface before now (triggered by reorientation) should simply change extents

    _unityAppReady = true;

    // why we skip present:
    // this will be the first frame to draw, so Start methods will be called
    // and we want to properly handle resolution request in Start (which might trigger surface recreate)
    // NB: we want to draw right after showing window, to avoid black frame creeping in

    _skipPresent = true;

    if (!UnityIsPaused())
        UnityRepaint();

    _skipPresent = false;
    [self repaint];

    [UIView setAnimationsEnabled: YES];
}

- (void)transitionToViewController:(UIViewController*)vc
{
    [self willTransitionToViewController: vc fromViewController: _rootController];

    // first: hide window and remove view hierarchy
    _window.hidden = YES; _rootController.view = nil; _window.rootViewController = nil;
    // second: assign new root controller (and view hierarchy with that) and show it
    _rootController = _window.rootViewController = vc; _rootController.view = _rootView; _window.hidden = NO;
    // third: restore window as key and layout subviews to finalize size changes
    [_window makeKeyAndVisible];
    [_window layoutSubviews];
}

#if UNITY_SUPPORT_ROTATION
- (void)interfaceWillChangeOrientationTo:(UIInterfaceOrientation)toInterfaceOrientation
{
    UIInterfaceOrientation fromInterfaceOrientation = _curOrientation;

    _curOrientation = toInterfaceOrientation;
    [_unityView willRotateToOrientation: toInterfaceOrientation fromOrientation: fromInterfaceOrientation];
}

- (void)interfaceDidChangeOrientationFrom:(UIInterfaceOrientation)fromInterfaceOrientation
{
    [_unityView didRotate];
}

#endif

@end


#if UNITY_SUPPORT_ROTATION

@implementation UnityAppController (OrientationSupport)
- (UIViewController*)createRootViewControllerForOrientation:(UIInterfaceOrientation)orientation
{
    NSAssert(orientation != 0, @"Bad UIInterfaceOrientation provided");
    if (_viewControllerForOrientation[orientation] == nil)
        _viewControllerForOrientation[orientation] = [self createUnityViewControllerForOrientation: orientation];
    return _viewControllerForOrientation[orientation];
}

- (bool)isCurrentInterfaceOrientationEnabled
{
    NSUInteger supportedOrientMask = EnabledAutorotationInterfaceOrientations();
    NSUInteger currOrientMask = (1 << _curOrientation);
    return (supportedOrientMask & currOrientMask) != 0;
}

- (void)forceAutorotatingControllerToRefreshEnabledOrientationsIfNeeded
{
    if (!UnityShouldAutorotate())
        return;

    if (![self isCurrentInterfaceOrientationEnabled])
    {
        // if the current orientation is disabled, attemptRotationToDeviceOrientation
        // does nothing because we're trying to rotate the view controller away from
        // the current device orientation
        [self transitionToViewController: [self createSecondaryAutorotatingViewController]];
    }
    else
        [UIViewController attemptRotationToDeviceOrientation];
}

- (void)checkOrientationRequest
{
    if (UnityHasOrientationRequest())
    {
        if (UnityShouldAutorotate())
        {
            if (_rootController != _viewControllerForOrientation[0])
            {
                [self transitionToViewController: [self createRootViewController]];
                [UIViewController attemptRotationToDeviceOrientation];
            }
            else
                [self forceAutorotatingControllerToRefreshEnabledOrientationsIfNeeded];
        }
        else
        {
            ScreenOrientation requestedOrient = (ScreenOrientation)UnityRequestedScreenOrientation();
            [self orientInterface: ConvertToIosScreenOrientation(requestedOrient)];
        }
        UnityOrientationRequestWasCommitted();
    }
}

- (void)orientInterface:(UIInterfaceOrientation)orient
{
    if (_curOrientation == orient && _rootController != _viewControllerForOrientation[0])
        return;

    if (_unityAppReady)
        UnityFinishRendering();

    [KeyboardDelegate StartReorientation];

    [CATransaction begin];
    {
        UIInterfaceOrientation oldOrient = _curOrientation;
        UIInterfaceOrientation newOrient = orient;

        [self interfaceWillChangeOrientationTo: newOrient];
        [self transitionToViewController: [self createRootViewControllerForOrientation: newOrient]];
        [self interfaceDidChangeOrientationFrom: oldOrient];

        [UIApplication sharedApplication].statusBarOrientation = orient;
    }
    [CATransaction commit];

    [KeyboardDelegate FinishReorientation];
}

- (void)orientUnity:(UIInterfaceOrientation)orient
{
    [self orientInterface: orient];
}

@end

#endif
