using UnityEngine;

public class GetAtt : MonoBehaviour {
    private void Awake() {
        RequestAtt();
    }

    private void RequestAtt() {
    #if UNITY_IOS && !UNITY_EDITOR
        var currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;
        Debug.Log($"Current authorization status: {currentStatus.ToString()}");

        switch (currentStatus) {
            case AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED:
                Debug.Log("Requesting authorization...");
                AppTrackingTransparency.RequestTrackingAuthorization();
                break;
            case AppTrackingTransparency.AuthorizationStatus.RESTRICTED:
            case AppTrackingTransparency.AuthorizationStatus.DENIED:
            case AppTrackingTransparency.AuthorizationStatus.AUTHORIZED:
                break;
        }
    #endif
    }
}
