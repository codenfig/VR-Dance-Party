#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Applies project-level settings; actual iOS build still requires Xcode on macOS.
public static class ConfigureIOSBuild
{
    [MenuItem("VR Dance Party/Configure iPhone Build Settings")]
    public static void Configure()
    {
        PlayerSettings.companyName = "VR Dance Party";
        PlayerSettings.productName = "VR Dance Party";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.codenfig.vrdanceparty");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        Debug.Log("iPhone settings applied. Select iOS in Build Settings / Build Profiles, set your Apple Team ID and signing in Xcode, then build. Confirm Unity and Xcode versions are compatible.");
    }
}
#endif
