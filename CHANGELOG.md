# Varjo XR Unity Plugin Changelog

All notable changes to this package are documented in this file.

## 2.3.0

Compatible with Varjo Base 3.4 or newer.

### Added

- Proper error handling
- Support for controller haptics
- Function to get Varjo session pointer
- New hasNewFrame boolean for mixed reality streams to inform if the stream has received a new frame since the last GetFrame
- Inline documentation for public methods and custom types
- Documentation link in the Package Manager

### Changed

- Invalid float parameter in VarjoRendering setting functions is now returning an error instead of silently clamping the value
- Fixed foveated rendering is now used as a fallback when foveated rendering is enabled but no gaze data is available
- By default use eye tracking data poll rate equal to fastest rate supported by connected headset (i.e. 200Hz for XR-3, VR-3 devices, and 100Hz for earlier devices)

### Removed

- Removed unit tests from the plugin package

### Fixed

- Fixed eye tracking not working if "Allow eye tracking" was enabled in Varjo Base after the plugin was loaded
- Fixed VarjoTextureBuffer using deprecated Texture2D.Resize() on Unity 2021.2 or newer

## 2.2.1

Compatible with Varjo Base 3.3.1 or newer.

### Fixed

- Fixed occlusion mask being upside down when using HDRP with Unity 2019

## 2.2.0

Compatible with Varjo Base 3.3 or newer.

### Removed

- Removed experimental 3D reconstruction support

### Fixed

- Fixed warped frame after calling XRGeneralSettings.Instance.Manager.StopSubsystems()
- Fixed GUIDs in tests conflicting with other XR SDK plugins
- Fixed VST mask in HDRP samples for Unity 2020 and newer

## 2.1.1

Compatible with Varjo Base 3.2 or newer.

### Fixed

- Fixed VarjoRendering.GetOcclusionMeshEnabled() always returning false
- Fixed Vive trackers not working correctly as hand held controller devices in builds
- Fixed Vive controllers sometimes being assigned to wrong hands

## 2.1.0

Compatible with Varjo Base 3.1 or newer.

### Changed

- Renamed input features to match common bindings

## 2.0.1

Compatible with Varjo Base 3.0 or newer.

### Fixed

- Fixed devices not working correctly with new input system

## 2.0.0

Compatible with Varjo Base 3.0 or newer.

### Added

- Support for Varjo XR-3 and VR-3
- Support for foveated rendering. Foveated rendering is now enabled by default
- Chroma key support
- Support for streaming VST camera metadata without DistortedColorStream texture buffers
- Face-lock support
- Runtime functions for rendering settings in VarjoRendering
- Support for SteamVR trackers
- Support for polling eye tracking data 200Hz outside the input subsystem
- Functions for controlling gaze output frequency and gaze output filter type
- XR Meshing subsystem (experimental) for XR-3 point cloud data

### Changed

- Refactored VarjoSettings and made them modifiable only in editor when the application is not running
- OpenVR controllers are now using predicted pose
- VarjoMixedReality.EnableDepthEstimation() doesn't require manually enabling 'depth sorting' setting anymore
- VarjoEyeTracking.RequestGazeCalibration() now takes only one parameter of type GazeCalibrationMode
- Updated samples for new features and other minor improvements
- Updated license

### Fixed

- Fixed package installation issues
- Fixed incorrect controller poses when tracking origin and direction has been overridden from Varjo Base
- Fixed culling issues
- Fixed Varjo.IsHMDConnected() and VarjoMixedReality.IsMRAvailable() returning false if 'Allow eye tracking' is disabled from Varjo Base
- Fixed crash when display subsystem stopped and started again
- Fixed controller tracking state not reported correctly
- Fixed HDRP stereo rendering for Unity 2019.4


## 1.0.0

Initial plugin release.

Compatible with Varjo Base 2.4 or newer.
