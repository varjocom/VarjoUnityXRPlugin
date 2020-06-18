# About Varjo XR Plugin

Varjo XR Plugin provides provides display and input support for Varjo HMDs.

## XR plugin systems

### Display

The display subsystem provides stereo rendering support for the XR Plugin.

### Input

The input subsystem provides tracking for the HMD and hand controllers, button inputs from the HMD and contollers and eye tracking. 

## Getting started

A step-by-step guide for adding Varjo HMD support in a project using HDRP:
- Open a HDRP project or create a new one using the HDRP project template.
- In 'Project Settings' -> 'XR Plug-in Management', Select 'Varjo' from the list of Plug-in providers.
- Select the main camera, and add 'Tracked Pose Driver' component to it. Make sure it is tracking the 'Center Eye - HMD reference' pose of 'Generic XR Device'.

## Varjo Settings

The following settings are available in the VarjoSettings asset:

### Separate Cull Pass For Focus Displays

If this is selected, Unity will perform an extra culling pass for the focus displays. Otherwise the culling results for the context displays will be reused for focus displays (this is the default, and is typically recommended).

### Stereo Rendering Mode

Selects the rendering mode:

#### Multi Pass

The scene is rendered in 4 separate passes, one for each view (left context, right context, left focus, right focus).

#### Two Pass

The scene is rendered in 2 passes: one 2-wide instanced stereo rendering pass for context displays and another for the focus displays.

### Context Scaling Factor

A scaling factor that can be used to reduce the rendering resolution of the context display surfaces.

### Focus Scaling Factor

A scaling factor that can be used to reduce the rendering resolution of the focus display surfaces.

### Flip Y

If selected, the rendering results will be flipped upside down before submitting them to the compositor. Select this if your scene appears upside down in the HMD.

### Submit Depth

If selected, the application will pass depth surfaces to the compositor (alongside the color surfaces). This allows the compositor to use Positional TimeWarp to improve rendering quality.

### Opaque

This checkbox can be used to tell the Varjo Compositor that the submitted surfaces are meant to be opaque and that the compositor should not perform any alpha blending against possible background applications when rendering the surfaces.

### Depth Sorting

This checkbox can be used to tell the Varjo Compositor that the application wants its contents to participate in per-pixel depth sorting: If other applications (or the Video See-Through view in XR headsets) have pixels closer to the camera than this application, they will get displayed instead of the pixels from this application. 'Submit Depth' should be enabled when using this option.

### Depth Test Range Enabled

If selected, the depth test range is enabled. Use Depth Test Near Z and Far Z to control the range inside which the depth test will be evaluated.

### Depth Test Near Z

Minimum depth included in the depth test range.

### Depth Test Far Z

Maximum depth included in the depth test range.

### Session Priority

This priority value is used when multiple clients are running at the same time. Sessions with higher priority render on top of lower ones.

## Developer documentation

For more information about the Varjo XR Plugin visit [developer.varjo.com](https://developer.varjo.com/docs/unity-xr-sdk/unity-xr-sdk).