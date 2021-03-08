# Varjo XR Unity Plugin Changelog

All notable changes to this package are documented in this file.

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
