// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public enum EventType
    {
        EVENT_VISIBILITY = 1,
        EVENT_BUTTON = 2,
        EVENT_HEADSET_STANDBY_STATUS = 6,
        EVENT_FOREGROUND = 7,
        EVENT_MR_DEVICE_STATUS = 8,
        EVENT_MR_CAMERA_PROPERTY_CHANGE = 9,
        EVENT_DATA_STREAM_START = 10,
        EVENT_DATA_STREAM_STOP = 11,
    }
    public enum MRDeviceStatus : long
    {
        Connected = 1,
        Disconnected = 2,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventBase
    {
        public ulong type;
        public long timeStamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventVisibility
    {
        public uint visible;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventButton
    {
        public uint pressed;
        public byte buttonId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventHeadsetStandbyStatus
    {
        public uint onStandby;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventForeground
    {
        public uint isForeground;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventMRDeviceStatus
    {
        public MRDeviceStatus status;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventMRCameraPropertyChange
    {
        public VarjoCameraPropertyType type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventDataStreamStart
    {
        public long streamId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventDataStreamStop
    {
        public long streamId;
    }

    public class VarjoEventManager : MonoBehaviour
    {
        private static VarjoEventManager _instance = null;
        public static VarjoEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Varjo Event Manager")
                    {
                        hideFlags = HideFlags.DontSave
                    };
                    Instance = go.AddComponent<VarjoEventManager>();
                }
                return _instance;
            }
            private set { _instance = value; }
        }

        public delegate void VisibilityEvent(bool visible);
        public static event VisibilityEvent OnVisibilityEvent;

        public delegate void StandbyEvent(bool onStandby);
        public static event StandbyEvent OnStandbyEvent;

        public delegate void ForegroundEvent(bool onForeground);
        public static event ForegroundEvent OnForegroundEvent;

        public delegate void MRDeviceStatusEvent(bool connected);
        public static event MRDeviceStatusEvent OnMRDeviceStatusEvent;

        public delegate void MRCameraPropertyChangeEvent(VarjoCameraPropertyType type);
        public static event MRCameraPropertyChangeEvent OnMRCameraPropertyChangeEvent;

        public delegate void DataStreamStartEvent(long streamId);
        public static event DataStreamStartEvent OnDataStreamStartEvent;

        public delegate void DataStreamStopEvent(long streamId);
        public static event DataStreamStopEvent OnDataStreamStopEvent;

        [DllImport("VarjoUnityXR")]
        private static extern IntPtr GetVarjoSession();

        [DllImport("VarjoUnityXR")] public static extern bool PollEvent(ref ulong eventType);
        [DllImport("VarjoUnityXR")] public static extern EventVisibility GetEventVisibility();
        [DllImport("VarjoUnityXR")] public static extern EventButton GetEventButton();
        [DllImport("VarjoUnityXR")] public static extern EventHeadsetStandbyStatus GetEventHeadsetStandbyStatus();
        [DllImport("VarjoUnityXR")] public static extern EventForeground GetEventForeground();
        [DllImport("VarjoUnityXR")] public static extern EventMRDeviceStatus GetEventMRDeviceStatus();
        [DllImport("VarjoUnityXR")] public static extern EventMRCameraPropertyChange GetEventMRCameraPropertyChange();
        [DllImport("VarjoUnityXR")] public static extern EventDataStreamStart GetEventDataStreamStart();
        [DllImport("VarjoUnityXR")] public static extern EventDataStreamStop GetEventDataStreamStop();

        /// <summary>
        /// Last polled event type
        /// </summary>
        private ulong eventType = 0;

        /// <summary>
        /// List of events stored this frame.
        /// </summary>
        private List<EventButton> buttonEvents = new List<EventButton>();

        public VarjoEventManager()
        {
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogError("Multiple instances of VarjoEventManager. Destroying the new one.");
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (GetVarjoSession() == IntPtr.Zero)
            {
                return;
            }

            // Update events
            buttonEvents.Clear();

            while (PollEvent(ref eventType))
            {
                switch ((EventType)eventType)
                {
                    // Keep track of application visibility and standby status
                    // and enable and disable rendering based on that
                    case EventType.EVENT_VISIBILITY:
                        bool layerVisible = GetEventVisibility().visible != 0;
                        if (OnVisibilityEvent != null)
                            OnVisibilityEvent(layerVisible);
                        break;

                    case EventType.EVENT_HEADSET_STANDBY_STATUS:
                        bool inStandBy = GetEventHeadsetStandbyStatus().onStandby != 0;
                        if (OnStandbyEvent != null)
                            OnStandbyEvent(inStandBy);
                        break;

                    case EventType.EVENT_FOREGROUND:
                        if (OnForegroundEvent != null)
                            OnForegroundEvent(GetEventForeground().isForeground != 0);
                        break;

                    // Update headset button states
                    case EventType.EVENT_BUTTON:
                        buttonEvents.Add(GetEventButton());
                        break;

                    case EventType.EVENT_MR_DEVICE_STATUS:
                        if (OnMRDeviceStatusEvent != null)
                            OnMRDeviceStatusEvent(GetEventMRDeviceStatus().status == MRDeviceStatus.Connected);
                        break;

                    case EventType.EVENT_MR_CAMERA_PROPERTY_CHANGE:
                        if (OnMRCameraPropertyChangeEvent != null)
                            OnMRCameraPropertyChangeEvent(GetEventMRCameraPropertyChange().type);
                        break;

                    case EventType.EVENT_DATA_STREAM_START:
                        if (OnDataStreamStartEvent != null)
                            OnDataStreamStartEvent(GetEventDataStreamStart().streamId);
                        break;

                    case EventType.EVENT_DATA_STREAM_STOP:
                        if (OnDataStreamStopEvent != null)
                            OnDataStreamStopEvent(GetEventDataStreamStop().streamId);
                        break;
                }
            }
        }

        /// <summary>
        /// Returns true when headset button gets pressed.
        /// </summary>
        /// <param name="buttonId">Id of headset button. 0 is application button.</param>
        /// <returns></returns>
        public bool GetButtonDown(int buttonId = 0)
        {
            for (int i = buttonEvents.Count - 1; i >= 0; --i)
            {
                if (buttonEvents[i].buttonId == buttonId)
                {
                    return buttonEvents[i].pressed != 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true when headset button gets released.
        /// </summary>
        /// <param name="buttonId">Id of headset button. 0 is application button.</param>
        /// <returns></returns>
        public bool GetButtonUp(int buttonId = 0)
        {
            for (int i = buttonEvents.Count - 1; i >= 0; --i)
            {
                if (buttonEvents[i].buttonId == buttonId)
                {
                    return buttonEvents[i].pressed == 0;
                }
            }

            return false;
        }
    }
}
