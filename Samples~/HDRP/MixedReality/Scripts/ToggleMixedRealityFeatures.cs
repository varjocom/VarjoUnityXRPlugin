using UnityEngine;

public class ToggleMixedRealityFeatures : MonoBehaviour
{
    public VarjoMR varjoMR;

    [Header("MR feature toggle keys")]
    public KeyCode MRToggleKey = KeyCode.Alpha1;
    public KeyCode depthEstimationToggleKey = KeyCode.Alpha2;
    public KeyCode reflectionToggleKey = KeyCode.Alpha3;
    public KeyCode VREyeOffsetToggleKey = KeyCode.Alpha4;
    public KeyCode DirectionalLightToggleKey = KeyCode.Alpha5;
    public GameObject directionalLight;



    void Start()
    {
        if (!varjoMR)
        {
            Debug.LogError("MixedReality not set. Disabling MixedRealityTest.");
            enabled = false;
        }
    }


    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(MRToggleKey))
            {
                varjoMR.videoSeeThrough = !varjoMR.videoSeeThrough;
            }
            if (Input.GetKeyDown(depthEstimationToggleKey))
            {
                varjoMR.depthEstimation = !varjoMR.depthEstimation;
            }
            if (Input.GetKeyDown(reflectionToggleKey))
            {
                varjoMR.environmentReflections = !varjoMR.environmentReflections;
            }
            if (Input.GetKeyDown(VREyeOffsetToggleKey))
            {
                if (varjoMR.VREyeOffset == 0f)
                {
                    varjoMR.VREyeOffset = 1.0f;
                }
                else
                {
                    varjoMR.VREyeOffset = 0f;
                }
            }

            if (Input.GetKeyDown(DirectionalLightToggleKey))
            {
                directionalLight.SetActive(!directionalLight.activeSelf);
            }
        }
    }
}