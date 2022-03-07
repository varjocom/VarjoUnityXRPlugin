using System;

using UnityEngine;
using UnityEngine.XR.Management;
using System.Runtime.InteropServices;

namespace Varjo.XR
{
    /// <summary>
    /// Error handling modes
    /// </summary>
    public enum VarjoErrorHandlingMode
    {
        /** <summary>Ignore all errors</summary> */
        Ignore,
        /** <summary>Log errors to debug out</summary> */
        Log,
        /** <summary>Throw errors</summary> */
        Throw
    }

    public static class VarjoError
    {
        /// <summary>
        /// Controls how errors are handled
        /// </summary>
        public static VarjoErrorHandlingMode ErrorHandlingMode { get; set; } = VarjoErrorHandlingMode.Log;

        /// <summary>
        /// Checks and reports error of last Varjo API call
        /// </summary>
        /// <returns>True if last API call completed successfully</returns>
        internal static bool CheckError()
        {
            long varjoErrorCode = GetError();

            if (varjoErrorCode != 0)
            {
                switch (ErrorHandlingMode)
                {
                    case VarjoErrorHandlingMode.Log:
                        Debug.LogWarning(GetErrorDescription(varjoErrorCode));
                        break;
                    case VarjoErrorHandlingMode.Throw:
                        throw new VarjoRuntimeException(varjoErrorCode, GetErrorDescription(varjoErrorCode));
                    default:
                        break;
                }
                return false;
            }
            return true;
        }

        internal static void HandleErrorMessage(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return;
            }

            switch (ErrorHandlingMode)
            {
                case VarjoErrorHandlingMode.Log:
                    Debug.LogWarning(errorMessage);
                    break;
                case VarjoErrorHandlingMode.Throw:
                    throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Gets human readable error string for given error code
        /// </summary>
        /// <returns>Error message</returns>
        private static string GetErrorDescription(long errorCode)
        {
            IntPtr ptr = GetErrorDesc(errorCode);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Gets error code of previous API call
        /// </summary>
        /// <returns>Error code</returns>
        [DllImport("VarjoUnityXR")]
        private static extern long GetError();

        /// <summary>
        /// Gets human readable error string for given error code
        /// </summary>
        /// <returns>Native pointer to error message</returns>
        [DllImport("VarjoUnityXR", CharSet = CharSet.Auto)]
        private static extern IntPtr GetErrorDesc(long errorCode);
    }

    /// <summary>
    /// Exception class thrown from API errors when Varjo.ErrorHandlingMode is ErrorHandlingMode.Throw
    /// </summary>
    public class VarjoRuntimeException : Exception
    {
        public VarjoRuntimeException(long error, string message)
            : base(message)
        {
            ErrorCode = error;
        }

        public long ErrorCode { get; private set; }
    }
}