# Mixed reality

This example shows how to use mixed reality features and real world reflections and lighting for virtual content.
In Post Process Volume there is VST white balance override that matches the exposure intensity to match exposure used by VST cameras. This makes the scene look more natural.

Note: To make the custom post process effect work, you have to enable it in Project Settings -> HDRP Default Settings -> Custom Post Process Orders -> After Post Process.

When real world reflections are enabled (3) the chrome sphere shows how much of the real world environment cube is captured. By looking around you can fill the missing parts.

## Mixed reality features

Use following keys:
1 - Toggles between VR and MR.
2 - Enables depth estimation to allow hands visibility in mixed reality.
3 - Enables real world reflections.
4 - Toggles between eyes position. In mixed reality the virtual cameras should be in slightly in different position because the VST cameras are in front of the headset.
5 - Toggles scene light on/off. When you use real world reflections your scene light may come from different direction that is real world light direction.


# Mixed reality masking

This example shows how to use masking to create a window into the real world from the virtual environment.

## SimpleMixedRealityExample

Use following keys:
Space - Toggle video see-through on/off


ESC quits demo if you build the scene.