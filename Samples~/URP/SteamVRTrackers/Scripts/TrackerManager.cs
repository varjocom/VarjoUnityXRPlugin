using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackerManager : MonoBehaviour
{
    [SerializeField] GameObject trackerPrefab;

    private Dictionary<string, GameObject> trackers = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        var devices = InputSystem.devices;
        foreach(var device in devices)
        {
            if (device.name.Contains("SteamVRTracker")) AddTracker(device);
        }
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!device.name.Contains("SteamVRTracker")) return;

        switch (change)
        {
            case InputDeviceChange.Added:
                // New Device found
                // Tracker roles "Held in hand (any)" and "Disabled" are not supported
                AddTracker(device);
                break;
            case InputDeviceChange.Disconnected:
                SetTrackerEnabled(device, false);
                break;
            case InputDeviceChange.Reconnected:
                SetTrackerEnabled(device, true);
                break;
            case InputDeviceChange.UsageChanged:
                Initialize(device);
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely
                // By default, Devices stay in the system once discovered
                RemoveTracker(device);
                break;
            case InputDeviceChange.ConfigurationChanged:
                Initialize(device);
                break;
            default:
                break;
        }
    }

    void AddTracker(InputDevice device)
    {
        if (trackers.ContainsKey(device.name)) return;

        Debug.Log($"New tracker added! : {device.displayName}");
        var trackerObject = Instantiate(trackerPrefab);
        trackers.Add(device.name, trackerObject);
        Initialize(device);
    }

    void RemoveTracker(InputDevice device)
    {
        if (trackers.ContainsKey(device.name))
        {
            Destroy(trackers[device.name]);
            trackers.Remove(device.name);
        }
    }

    void SetTrackerEnabled(InputDevice device, bool state)
    {
        if (trackers.ContainsKey(device.name))
        {
            trackers[device.name].SetActive(state);
        } else if (state)
        {
            AddTracker(device);
        }
    }

    void Initialize(InputDevice device)
    {
        if (trackers.ContainsKey(device.name))
        {
            var tracker = trackers[device.name].GetComponent<Tracker>();
            if (tracker) tracker.Initialize(device);
        }
        else
        {
            AddTracker(device);
        }
    }
}
