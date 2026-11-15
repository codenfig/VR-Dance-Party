# iPhone 16 + Intel Mac Pro — iOS build guide

This repository contains Unity source scripts for a **phone-based stereoscopic VR Box prototype**. It is not an Xcode project or a prebuilt IPA.

## Requirements
- Intel Mac Pro with a version of macOS supported by the chosen **Unity Editor and Xcode**. Check both vendors' current system requirements before installing: older Intel Macs may not support the newest macOS/Xcode.
- Unity Hub and a Unity version that supports iOS, with **iOS Build Support** installed.
- Xcode and its command-line tools.
- iPhone 16, compatible VR Box viewer, USB cable, and an Apple Account for signing. The iPhone may need Developer Mode enabled.
- For the existing prototype's `Input.gyro` and `Input.GetAxis` APIs, set **Active Input Handling** to **Both** or **Input Manager (Old)**.

## Build instructions
1. Clone the repository, create a **3D (Built-In Render Pipeline)** Unity project, and copy the repository's `Assets/Editor` and `Assets/Scripts` into its `Assets` folder.
2. In Unity, select **VR Dance Party > Create First Playable Scene**. This generates `Assets/Scenes/VRDanceParty_FirstScene.unity`.
3. Select **VR Dance Party > Configure iPhone Build Settings**.
4. Open **File > Build Settings** (or **Build Profiles**), select **iOS**, and choose **Switch Platform**. Make sure the generated scene is included.
5. In **Project Settings > Player > iOS**, verify the bundle identifier `com.codenfig.vrdanceparty` is unique for your signing team; adjust if necessary.
6. Click **Build** to generate an Xcode project in a separate output folder.
7. Open the generated `.xcodeproj` or `.xcworkspace` in Xcode. Under **Signing & Capabilities**, choose your Apple development team and enable automatic signing.
8. Connect and trust your iPhone, enable Developer Mode if prompted, select it as the run destination, and press **Run**.
9. Test the stereo display, head movement, gaze menu, audio and thermal behavior with the phone **outside** the VR Box first.
10. Place the phone in the VR Box only when the view is comfortable and stable. Stay seated.

## Current limitations and troubleshooting
- **Not tested on iPhone 16:** Gyroscope coordinate conversion is currently shared with the Android prototype and may need platform-specific adjustment after real-device testing.
- **Optics:** Basic side-by-side rendering does not implement lens distortion correction or device-specific IPD calibration. Some VR Box viewers may be uncomfortable or incompatible.
- **Controls:** Gaze buttons are available after regenerating the scene; phone touchscreen controls are hard to access while the phone is inside the viewer.
- **Build errors:** If Unity's iOS build target is missing, install iOS Build Support in Unity Hub. If Xcode cannot install/run, check macOS/Xcode/iOS compatibility and signing.
- **Unity API:** `NamedBuildTarget.iOS` requires a compatible Unity version (Unity 2022.3 LTS or newer recommended). Newer Unity versions may expose the build UI as Build Profiles.
- **Safety:** This is a stationary 3DoF prototype. Do not walk while using it; stop if you experience discomfort.
- **Distribution:** An Apple Account may support personal device development signing. App Store distribution requires enrollment in the Apple Developer Program.

## Acceptance checks
- iPhone launches in landscape.
- Left and right views are side by side.
- Head turning changes view with tolerable latency.
- Gaze selection works for Pause/Play and Dance Style.
- Music and dancer movement remain synchronized during normal playback.
- The phone remains within acceptable temperature/performance limits during a short seated session.
