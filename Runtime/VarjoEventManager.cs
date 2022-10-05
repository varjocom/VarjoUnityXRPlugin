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
    /// <summary>
    /// Event Type
    /// </summary>
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
        EVENT_MR_CHROMA_KEY_CONFIG_CHANGE = 13,
    }

    /// <summary>
    /// MR Device Status
    /// </summary>
    public enum MRDeviceStatus : long
    {
        Connected = 1,
        Disconnected = 2,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventBase
    {
        /** <summary>Type of the event.</summary> */
        public ulong type;
        /** <summary>Timestamp of the time when the event was issued.</summary> */
        public long timeStamp;
    }

    /// <summary>
    /// Visibility event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventVisibility
    {
        /** <summary>Current visibility.</summary> */
        public uint visible;
    }

    /// <summary>
    /// Button event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventButton
    {
        /** <summary>Is the button pressed down.</summary> */
        public uint pressed;
        /** <summary>Id of the button.</summary> */
        public byte buttonId;
    }

    /// <summary>
    /// Standby status event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventStandbyStatus
    {
        /** <summary>Is the headset on standby.</summary> */
        public uint onStandby;
    }

    /// <summary>
    /// Foreground status event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventForeground
    {
        public uint isForeground;
    }

    /// <summary>
    /// Mixed reality device status event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventMRDeviceStatus
    {
        /** <summary>Is there a mixed reality capable device connected.</summary> */
        public MRDeviceStatus status;
    }

    /// <summary>
    /// Camera property change event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventMRCameraPropertyChange
    {
        /** <summary>Changed property.</summary> */
        public VarjoCameraPropertyType type;
    }

    /// <summary>
    /// Data stream start event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventDataStreamStart
    {
        /** <summary>Stream id.</summary> */
        public long streamId;
    }

    /// <summary>
    /// Data stream stop event data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EventDataStreamStop
    {
        /** <summary>Stream id.</summary> */
        public long streamId;
    }

    /// <summary>
    /// Varjo Event Manager
    /// </summary>
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

        public delegate void MRChromaKeyConfigChangeEvent();
        public static event MRChromaKeyConfigChangeEvent OnMRChromaKeyConfigChangeEvent;

        /// <summary>
        /// Poll Event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>True if has event.</returns>
        [DllImport("VarjoUnityXR")] public static extern bool PollEvent(ref ulong eventType);
        /// <summary>
        /// Get Visibility Data of polled event.
        /// </summary>
        /// <returns>Visibility of polled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventVisibility GetEventVisibility();
        /// <summary>
        /// Get button data of polled event.
        /// </summary>
        /// <returns>EventButton of polled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventButton GetEventButton();
        /// <summary>
        /// Get EventStandbyStatus.
        /// </summary>
        /// <returns>EventStandbyStatus of polled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventStandbyStatus GetEventStandbyStatus();
        /// <summary>
        /// Get EventForeground.
        /// </summary>
        /// <returns>EventForeground of polled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventForeground GetEventForeground();
        /// <summary>
        /// Get EventMRDeviceStatus.
        /// </summary>
        /// <returns>EventMRDeviceStatus of ppolled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventMRDeviceStatus GetEventMRDeviceStatus();
        /// <summary>
        /// Get EventMRCameraPropertyChange
        /// </summary>
        /// <returns>EventMRCameraPropertyChange of polled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventMRCameraPropertyChange GetEventMRCameraPropertyChange();
        /// <summary>
        /// Get EventDataStreamStart
        /// </summary>
        /// <returns>EventDataStreamStart of polled event or default value.</returns>
        [DllImport("VarjoUnityXR")] public static extern EventDataStreamStart GetEventDataStreamStart();
        /// <summary>
        /// Get EventDataStreamStop
        /// </summary>
        /// <returns>EventDataStreamStop of polled event or default value.</returns>
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
            if (Varjo.GetVarjoSession() == IntPtr.Zero)
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
                        bool inStandBy = GetEventStandbyStatus().onStandby != 0;
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

                    case EventType.EVENT_MR_CHROMA_KEY_CONFIG_CHANGE:
                        if (OnMRChromaKeyConfigChangeEvent != null)
                            OnMRChromaKeyConfigChangeEvent();
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
