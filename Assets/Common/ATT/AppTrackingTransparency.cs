using System;
using UnityEngine;

public static class AppTrackingTransparency {
#if UNITY_IOS
    private delegate void AppTrackingTransparencyCallback(int result);

    [AOT.MonoPInvokeCallback(typeof(AppTrackingTransparencyCallback))]
    private static void AppTrackingTransparencyCallbackReceived(int result) {
        // Force to use Default Synchronization context to run callback on Main Thread
        System.Threading.Tasks.Task.Delay(1)
            .ContinueWith((unused) => {
                if (OnAuthorizationRequestDone != null) {
                    switch (result) {
                        case 0:
                            OnAuthorizationRequestDone(AuthorizationStatus.NOT_DETERMINED);
                            break;
                        case 1:
                            OnAuthorizationRequestDone(AuthorizationStatus.RESTRICTED);
                            break;
                        case 2:
                            OnAuthorizationRequestDone(AuthorizationStatus.DENIED);
                            break;
                        case 3:
                            OnAuthorizationRequestDone(AuthorizationStatus.AUTHORIZED);
                            break;
                        default:
                            OnAuthorizationRequestDone(AuthorizationStatus.NOT_DETERMINED);
                            break;
                    }
                }
            }, currentSynchronizationContext);
    }

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void requestTrackingAuthorization(AppTrackingTransparencyCallback callback);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int trackingAuthorizationStatus();
#endif

    private static System.Threading.Tasks.TaskScheduler currentSynchronizationContext;

    static AppTrackingTransparency() {
        currentSynchronizationContext = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
    }

    /// <summary>
    /// Possible App Tracking Transparency authorization status 
    /// </summary>
    public enum AuthorizationStatus {
        NOT_DETERMINED,

        /// <summary>
        /// User restrited app tracking. IDFA not available
        /// </summary>
        RESTRICTED,

        /// <summary>
        /// User did not grant access to IDFA
        /// </summary>
        DENIED,

        /// <summary>
        /// You can safely request IDFA
        /// </summary>
        AUTHORIZED
    };

    /// <summary>
    /// Callback invoked once user made a decision through iOS App Tracking Transparency native popup
    /// </summary>
    public static Action<AuthorizationStatus> OnAuthorizationRequestDone;

    /// <summary>
    /// Obtain current Tracking Authorization Status
    /// </summary>
    public static AuthorizationStatus TrackingAuthorizationStatus {
        get {
        #if UNITY_EDITOR
            return AuthorizationStatus.AUTHORIZED;
        #elif UNITY_IOS
            return (AuthorizationStatus) trackingAuthorizationStatus();
        #else
            return AuthorizationStatus.NOT_DETERMINED;
        #endif
        }
    }

    public static void RequestTrackingAuthorization() {
    #if UNITY_EDITOR
        Debug.Log("Running on Editor platform. Callback invoked with debug result");
        OnAuthorizationRequestDone?.Invoke(AuthorizationStatus.AUTHORIZED);
   #elif UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            requestTrackingAuthorization(AppTrackingTransparencyCallbackReceived);
        } else {
            Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
        }
    #else
            Debug.Log(string.Format("Platform '{0}' not supported", Application.platform));
    #endif
    }

    public static string GetIdfa() {
    #if UNITY_EDITOR
        return "unity-idfa";
    #elif UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                var idfa = UnityEngine.iOS.Device.advertisingIdentifier;
                return string.IsNullOrEmpty(idfa) ? null : idfa;
            } else {
                return null;
            }
    #else
            return null;
    #endif
    }
}