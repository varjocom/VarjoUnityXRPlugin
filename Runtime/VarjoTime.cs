using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    public class VarjoTime
    {
        /// <summary>
        /// Gets current Varjo system timestamp.
        /// </summary>
        /// <remarks>
        /// Returned timestamp is from a realtime monotonic clock. Its epoch is Varjo system startup specific
        /// and is not affected by time-of-day settings.
        /// </remarks>
        /// <returns>Nanoseconds since Varjo system epoch.</returns>
        public static long GetVarjoTimestamp() { return Native.GetCurrentTimestamp(); }

        /// <summary>
        /// Unix epoch constant
        /// </summary>
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts Varjo system timestamp to DateTime object
        /// </summary>
        /// <remarks>
        /// Conversion is done by running synchronization of Varjo clock with operating system's time-of-day clock.
        /// Note, conversion will fail if requested for a timestamp value preceding the Varjo system launch or if the
        /// input value is too old vs current Varjo time. Please check Varjo documentation for more information.
        /// When conversion fails, error is raised and handled according to settings in VarjoError class and in case
        /// VarjoError.ErrorHandlingMode != VarjoErrorHandlingMode.Throw null is returned.
        /// </remarks>
        /// <returns>DateTime object if conversion was possible or null on failure.</returns>
        public static DateTime? ConvertVarjoTimestampToDateTime(long varjoTimestamp)
        {
            long nanosecondsSinceUnixEpoch = Native.ConvertVarjoTimestampToUnixTime(varjoTimestamp);
            if (!VarjoError.CheckError())
            {
                return null;
            }

            return UnixEpoch.AddTicks(nanosecondsSinceUnixEpoch / 100);
        }

        /// <summary>
        /// Native interface functions
        /// </summary>
        private class Native
        {
            [DllImport("VarjoUnityXR")]
            public static extern long GetCurrentTimestamp();

            [DllImport("VarjoUnityXR")]
            public static extern long ConvertVarjoTimestampToUnixTime(long varjoTimestamp);
        }
    }
}