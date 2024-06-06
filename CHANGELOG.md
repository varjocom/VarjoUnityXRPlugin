# Varjo XR Unity Plugin Changelog

All notable changes to this package are documented in this file.

## 3.7.0

Compatible with Varjo Base 4.3 or newer.
Add preliminary support for Unity 6.0 or newer

- Upgrade com.unity.xr.arfoundation dependency to 5.1.3
- Compatible with com.unity.xr.arfoundation 6.0.0

## 3.6.0

Compatible with Varjo Base 4.0 or newer.

### Added

 - Support for Varjo Controllers
 - Support for XR-4
 - Added Varjo.XR.AutoExposureBehavior property

### Changed

 - Upgrade to oldest currently supported Unity LTS version 2021.3
  - Upgrade com.unity.xr.management dependency to 4.4.0
  - Upgrade com.unity.xr.arfoundation dependency to 5.1.0

### Fixed

 - Fixed Native-To-Managed callbacks for VarjoMixedReality.StartDataStream missing MonoPInvokeCallback attribute

## 3.5.0

Compatible with Varjo Base 3.10 or newer.

### Changed

- Deprecated and removed support for Legacy 10-dot gaze calibration. Newer Fast
  5-dot gaze calibration allows to achieve higher accuracy and precision.
  Requesting Legacy 10-dot gaze calibration will trigger Fast 5-dot calibration.

## 3.4.0

Compatible with Varjo Base 3.9 or newer.

### Changed

- Changed `VarjoEyeTracking.GetGaze` and `VarjoEyeTracking.GetEyeMeasurements`
  to return the really latest available data instead of the last data frame in
  the previous call to `VarjoEyeTracking.GetGazeList` or the previous Unity XR
  Input update.

## 3.3.0

Compatible with Varjo Base 3.8 or newer.

### Added

- Support for retrieving and controlling headset interpupillary distance
  and its adjustment (see `VarjoHeadsetIPD` class)
- Support for user presence status (`UnityEngine.XR.CommonUsages.userPresence`)
- Support for getting current Varjo system timestamp (`VarjoTime.GetVarjoTimestamp`)
  and support converting that timestamp to DateTime structure (`VarjoTime.ConvertVarjoTimestampToDateTime`)

### Fixed

- Fixed build post processing being performed when building for unsupported target platforms

### Changed

- Varjo VR-1, VR-2, VR-2 Pro and XR-1 Developer Edition are no longer supported (these devices are not supported with Varjo Base 3.8 or newer)

### Removed

- Removed YUV422 color stream conversion functions for XR-1 Developer Edition

## 3.2.0

Compatible with Varjo Base 3.7 or newer.

### Added

- Support for new environment cubemap mode
- Support for blend control mask (D3D11 only)
- Added new metadata to environment cubemap stream
- Eye openness data to `VarjoEyeTracking.EyeMeasurements` structure
- Added support for cancelling gaze calibration with
  `VarjoEyeTracking.CancelGazeCalibration` function.
- Added new `VarjoEyeTracking.HeadsetAlignmentGuidanceMode` option to
  control whether gaze calibration user interface waits for input from
  user (headset button press) before starting calibration sequence.
- Added a new camera property to control the new eye reprojection feature.

### Changed

- Eye open amount data in `XR.Eyes` now uses actual eye openness tracking
results instead of gaze availability

## 3.1.1

### Added

- Device layouts for all supported SteamVR tracker roles

### Fixed

- Fixed memory layout of `VarjoEyeTracking.EyeMeasurements` structure

## 3.1.0

Compatible with Varjo Base 3.6 or newer.

### Added

- Support for up to 14 SteamVR trackers with button inputs
- Proper support for Valve Index controllers
- Device layouts for all supported input devices
- DirectX 12 support (requires Unity 2021.2 or newer)
- Function for controlling global chroma keying
- Support for triggering "One dot" gaze tracking calibration

### Changed

- Input subsystem now ensures there is only one SteamVR controller device with
a given role (left hand, right foot, keyboard etc.) and reports other devices
with the same role as generic tracker devices without button inputs
- Tracked devices have more descriptive names now

### Fixed

- Fixed Session subsystem reporting wrong installed status
- Fixed `VarjoCameraSubsystem.MetadataStream` not working with IL2CPP

## 3.0.0

Compatible with Varjo Base 3.5 or newer.

### Added

- Support for simple two view stereo renderering
- AR Foundation support, including the following:
  - XRSessionSubsystem provider
  - XRCameraSubsystem provider, adding support for controlling video see-through cameras with `XRCameraSubsystem.Start()` and `XRCameraSubsystem.Stop()`,
  accessing the camera image CPU buffers, utilities for converting the buffers to RGB and grayscale textures and support for retrieving camera intrinsics
  - XROcclusionSubsystem provider, adding support for enabling and disabling environment depth estimation with `XROcclusionSubsystem.Start()` and `XROcclusionSubsystem.Stop()`
- New `VarjoEyeTracking.EyeMeasurements` structure, which provides gaze
  tracker's estimates for user's pupil and iris diamaters in millimeters, their
  ratios and user's interpupillary distance
- New `VarjoEyeTracking.GetEyeMeasurements()` function for retrieving latest
  eye measurements data and added overload for `VarjoEyeTracking.GetGazeList()`
  function for retrieving `EyeMeasurements` data together with `GazeData` data

### Changed

- Changed minimum supported Unity version to Unity 2020.3
- Moved camera metadata stream to `VarjoCameraSubsystem.MetadataStream`, where it can be accessed after calling `VarjoCameraSubsystem.EnableMetadataStream()`
- Renamed `EventHeadsetStandbyStatus` as `EventStandbyStatus` and `VarjoEventManager.GetEventHeadsetStandbyStatus()` as `VarjoEventManager.GetEventStandbyStatus()`

### Removed

- Removed deprecated `VarjoEyeTracking.RequestGazeCalibration(GazeCalibrationMode calibrationMode, GazeOutputFilterMode outputFilterMode)` and `GazeOutputFilterMode`
- Removed the old implementation of `VarjoDistortedColorStream` as the stream is now available through `VarjoCameraSubsystem`

### Deprecated

- Deprecated `leftPupilSize` and `rightPupilSize` fields of `VarjoEyeTracking.GazeData`
  structure. Change new code to use values provided in `VarjoEyeTracking.EyeMeasurements`
  structure.

### Fixed

- Fixed Varjo Loader Initialization always returning True
- Fixed inside-out tracking not working without SteamVR being installed

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
