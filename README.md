# About Varjo XR Plugin

Varjo XR Plugin provides provides display, input and mixed reality feature support for Varjo HMDs.

## XR plugin systems

### Display

The display subsystem provides stereo rendering support for the XR Plugin.

### Input

The input subsystem provides tracking for the HMD and hand controllers, button inputs from the HMD and contollers and eye tracking. 

### Session

The session subsystem provides information about mixed reality feature availability.

### Camera

The camera subsystem allows controlling video see-through cameras and accessing the camera streams. 

### Occlusion

The occlusion subsystem allows controlling the environment depth occlusion. 

## Getting started

- Create or open a Unity project. See [Unity XR SDK Compatibility page](https://developer.varjo.com/docs/unity-xr-sdk/compatibility) for compatible Unity editor and render pipeline versions.
- Open Window -> Package Manager.
- Click the '+' icon in the top left corner, select 'Add Package from git URL' and input the URL to the Varjo XR package repository: [https://github.com/varjocom/VarjoUnityXRPlugin.git](https://github.com/varjocom/VarjoUnityXRPlugin.git).
Alternatively, if you want to install the package from a local copy, you can select 'Add package from disk' and navigate to the package.json file inside the Varjo XR package.
- In 'Project Settings' -> 'XR Plug-in Management', Select 'Varjo' from the list of Plug-in providers.
- To set up tracking, select 'GameObject' -> 'XR' -> 'Convert Main Camera To XR Rig' from Unity’s main menu. For more information on how to migrate an existing Unity scene, see [Configuring your Unity Project for XR](https://docs.unity.cn/Manual/configuring-project-for-xr.html).

## Varjo Settings

The following settings are available for Varjo XR Plugin in 'Project Settings' -> 'XR Plug-in Management' -> 'Varjo'.

At runtime these settings can be changed using the functions in Varjo.XR.VarjoRendering.

### Stereo Rendering Mode

Selects the rendering mode:

#### Multi Pass

The scene is rendered in 4 separate passes, one for each view (left context, right context, left focus, right focus).

#### Two Pass

The scene is rendered in 2 passes: one 2-wide instanced stereo rendering pass for context displays and another for the focus displays.

#### Stereo

The scene is rendered in one two-wide instanced stereo rendering pass for the context displays. Focus views are not rendered and foveated rendering is disabled.

### Separate Cull Pass For Focus Displays

If this is selected, Unity will perform an extra culling pass for the focus displays. Otherwise the culling results for the context displays will be reused for focus displays (this is the default, and is typically recommended).

### Foveated Rendering

If selected, foveated rendering is enabled for the application.

### Context Scaling Factor

A scaling factor that can be used to reduce the rendering resolution of the context display surfaces.

### Focus Scaling Factor

A scaling factor that can be used to reduce the rendering resolution of the focus display surfaces.

### Opaque

This checkbox can be used to tell the Varjo Compositor that the submitted surfaces are meant to be opaque and that the compositor should not perform any alpha blending against possible background applications when rendering the surfaces.

### Face-locked

If selected, the compositor disables warping for the layer. Use only if the camera is face-locked.

### Flip Y

If selected, the rendering results will be flipped upside down before submitting them to the compositor. Select this if your scene appears upside down in the HMD.

### Occlusion Mesh

If selected, the occlusion mesh is enabled.

### Session Priority

This priority value is used when multiple clients are running at the same time. Sessions with higher priority render on top of lower ones.

### Submit Depth

If selected, the application will pass depth surfaces to the compositor (alongside the color surfaces). This allows the compositor to use positional timewarp to improve rendering quality.

### Depth Sorting

This checkbox can be used to tell the Varjo Compositor that the application wants its contents to participate in per-pixel depth sorting: If other applications (or the Video See-Through view in XR headsets) have pixels closer to the camera than this application, they will get displayed instead of the pixels from this application. 'Submit Depth' should be enabled when using this option.

### Depth Test Range Enabled

If selected, the depth test range is enabled. Use Depth Test Near Z and Far Z to control the range inside which the depth test will be evaluated.

### Depth Test Near Z

Minimum depth included in the depth test range.

### Depth Test Far Z

Maximum depth included in the depth test range.
## Developer documentation

For more information about the Varjo XR Plugin visit [developer.varjo.com](https://developer.varjo.com/docs/unity-xr-sdk/unity-xr-sdk).