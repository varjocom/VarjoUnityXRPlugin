// Copyright 2021 Varjo Technologies Oy. All rights reserved.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Varjo.XR
{
    class VarjoMathUtils
    {
        private static Matrix4x4 flipYZ = new Matrix4x4(
            new Vector4(1, 0, 0, 0), //
            new Vector4(0, -1, 0, 0), //
            new Vector4(0, 0, -1, 0), //
            new Vector4(0, 0, 0, 1) //
        );

        private static Matrix4x4 flipZ = new Matrix4x4(
            new Vector4(1, 0, 0, 0), //
            new Vector4(0, 1, 0, 0), //
            new Vector4(0, 0, -1, 0), //
            new Vector4(0, 0, 0, 1) //
        );

        private static Matrix4x4 reverseZ = new Matrix4x4(
            new Vector4(1, 0, 0, 0), //
            new Vector4(0, 1, 0, 0), //
            new Vector4(0, 0, -1, 0), //
            new Vector4(0, 0, 1, 1) //
        );

        public static Matrix4x4 WorldMatrixToUnity(double[] mat)
        {
            Debug.Assert(mat.Length == 16);
            return (flipZ * ConvertDoubleToFloatMatrix(mat) * flipZ);
        }

        public static Matrix4x4 ProjectionMatrixToUnity(double[] mat)
        {
            return reverseZ * ConvertDoubleToFloatMatrix(mat);
        }

        public static Matrix4x4 ExtrinsicsToUnity(VarjoMatrix extrinsics, VarjoMatrix streamTransform)
        {
            return flipYZ * ConvertDoubleToFloatMatrix(extrinsics.value) * flipYZ *
                   ConvertDoubleToFloatMatrix(streamTransform.value) * flipZ;
        }

        public static Matrix4x4 GetRectificationMatrix(VarjoMatrix extrinsics)
        {
            return flipYZ * ConvertDoubleToFloatMatrixRotation(extrinsics.value).inverse * flipYZ;
        }

        private static Matrix4x4 ConvertDoubleToFloatMatrixRotation(double[] mat)
        {
            Debug.Assert(mat.Length == 16);
            Matrix4x4 m = new Matrix4x4();
            for (int i = 0; i < 12; i++) m[i] = (float)mat[i];
            m[3, 3] = 1.0f;
            return m;
        }

        public static Matrix4x4 ConvertDoubleToFloatMatrix(double[] mat)
        {
            Debug.Assert(mat.Length == 16);
            Matrix4x4 m = new Matrix4x4();
            for (int i = 0; i < 16; i++) m[i] = (float)mat[i];
            return m;
        }
    }
}