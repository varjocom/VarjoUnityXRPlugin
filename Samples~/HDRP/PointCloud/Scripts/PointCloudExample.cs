#if VARJO_EXPERIMENTAL_FEATURES
using UnityEngine;
using Varjo.XR;

public class PointCloudExample : MonoBehaviour
{
    [Header("Reconstruction")]
    public VarjoReconstruction reconstruction;
    public bool reconstructionEnabled = false;

    [Header("Toggle reconstruction")]
    public KeyCode toggleKey = KeyCode.P;

    [Header("Change mesh queue size")]
    public KeyCode increaseQueueSizeKey = KeyCode.PageUp;
    public KeyCode decreaseQueueSizeKey = KeyCode.PageDown;

    [Header("Video see-through")]
    public bool vstEnabled = true;

    [Header("Toggle video see-through")]
    public KeyCode toggleVSTKey = KeyCode.V;

    private bool originalOpaqueValue;


    private void Start()
    {
        originalOpaqueValue = VarjoRendering.GetOpaque();

        UpdateReconstructionState();
        UpdateVSTState();
    }

    void Update()
    {
        if (reconstructionEnabled != reconstruction.IsRunning())
        {
            reconstructionEnabled = reconstruction.IsRunning();
            UpdateReconstructionState();
        }

        if (Input.GetKeyDown(toggleKey))
        {
            reconstructionEnabled = !reconstructionEnabled;
            UpdateReconstructionState();
        }

        if (Input.GetKeyDown(toggleVSTKey))
        {
            vstEnabled = !vstEnabled;
            UpdateVSTState();
        }

        if (Input.GetKeyDown(increaseQueueSizeKey))
        {
            reconstruction.meshQueueSize += 5;
        }

        if (Input.GetKeyDown(decreaseQueueSizeKey))
        {
            reconstruction.meshQueueSize = Mathf.Max(0, reconstruction.meshQueueSize - 5);
        }
    }

    public void UpdateReconstructionState()
    {
        if (reconstructionEnabled)
        {
            reconstruction.enabled = true;
        }
        else
        {
            reconstruction.enabled = false;
        }
    }

    public void UpdateVSTState()
    {
        if (vstEnabled)
        {
            VarjoRendering.SetOpaque(false);
            VarjoMixedReality.StartRender();

        }
        else
        {
            VarjoRendering.SetOpaque(true);
            VarjoMixedReality.StopRender();
        }
    }

    private void OnDisable()
    {
        VarjoRendering.SetOpaque(originalOpaqueValue);
    }
}
#endif