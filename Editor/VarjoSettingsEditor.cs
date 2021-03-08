using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Varjo.XR;

namespace Varjo.XR.Editor
{
    [CustomEditor(typeof(VarjoSettings))]
    public class VarjoSettingsEditor : UnityEditor.Editor
    {
        private const string k_StereoRenderingMode = "StereoRenderingMode";
        private const string k_SeparateCullPass = "SeparateCullPass";
        private const string k_FoveatedRendering = "FoveatedRendering";
        private const string k_ContextScalingFactor = "ContextScalingFactor";
        private const string k_FocusScalingFactor = "FocusScalingFactor";
        private const string k_Opaque = "Opaque";
        private const string k_FaceLocked = "FaceLocked";
        private const string k_FlipY = "FlipY";
        private const string k_OcclusionMesh = "OcclusionMesh";
        private const string k_SessionPriority = "SessionPriority";
        private const string k_SubmitDepth = "SubmitDepth";
        private const string k_DepthSorting = "DepthSorting";
        private const string k_DepthTestRange = "DepthTestRange";
        private const string k_DepthTestNearZ = "DepthTestNearZ";
        private const string k_DepthTestFarZ = "DepthTestFarZ";

        static GUIContent s_StereoRenderingModeLabel = EditorGUIUtility.TrTextContent("Stereo Rendering Mode");
        static GUIContent s_SeparateCullPassLabel = EditorGUIUtility.TrTextContent("Separate Cull Pass For Focus Display");
        static GUIContent s_FoveatedRenderingLabel = EditorGUIUtility.TrTextContent("Foveated Rendering");
        static GUIContent s_ContextScalingFactorLabel = EditorGUIUtility.TrTextContent("Context Scaling Factor");
        static GUIContent s_FocusScalingFactorLabel = EditorGUIUtility.TrTextContent("Focus Scaling Factor");
        static GUIContent s_OpaqueLabel = EditorGUIUtility.TrTextContent("Opaque");
        static GUIContent s_FaceLockedLabel = EditorGUIUtility.TrTextContent("Face-locked");
        static GUIContent s_FlipYLabel = EditorGUIUtility.TrTextContent("Flip Y");
        static GUIContent s_OcclusionMeshLabel = EditorGUIUtility.TrTextContent("Occlusion Mesh");
        static GUIContent s_SessionPriorityLabel = EditorGUIUtility.TrTextContent("Session Priority");
        static GUIContent s_SubmitDepthLabel = EditorGUIUtility.TrTextContent("Submit Depth");
        static GUIContent s_DepthSortingLabel = EditorGUIUtility.TrTextContent("Depth Sorting");
        static GUIContent s_DepthTestRangeLabel = EditorGUIUtility.TrTextContent("Depth Test Range Enabled");
        static GUIContent s_DepthTestNearZLabel = EditorGUIUtility.TrTextContent("Depth Test Near Z");
        static GUIContent s_DepthTestFarZLabel = EditorGUIUtility.TrTextContent("Depth Test Far Z");

        private SerializedProperty m_StereoRenderingMode;
        private SerializedProperty m_SeparateCullPass;
        private SerializedProperty m_FoveatedRendering;
        private SerializedProperty m_ContextScalingFactor;
        private SerializedProperty m_FocusScalingFactor;
        private SerializedProperty m_Opaque;
        private SerializedProperty m_FaceLocked;
        private SerializedProperty m_FlipY;
        private SerializedProperty m_OcclusionMesh;
        private SerializedProperty m_SessionPriority;
        private SerializedProperty m_SubmitDepth;
        private SerializedProperty m_DepthSorting;
        private SerializedProperty m_DepthTestRange;
        private SerializedProperty m_DepthTestNearZ;
        private SerializedProperty m_DepthTestFarZ;

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            if (m_StereoRenderingMode == null) m_StereoRenderingMode = serializedObject.FindProperty(k_StereoRenderingMode);
            if (m_SeparateCullPass == null) m_SeparateCullPass = serializedObject.FindProperty(k_SeparateCullPass);
            if (m_FoveatedRendering == null) m_FoveatedRendering = serializedObject.FindProperty(k_FoveatedRendering);
            if (m_ContextScalingFactor == null) m_ContextScalingFactor = serializedObject.FindProperty(k_ContextScalingFactor);
            if (m_FocusScalingFactor == null) m_FocusScalingFactor = serializedObject.FindProperty(k_FocusScalingFactor);
            if (m_Opaque == null) m_Opaque = serializedObject.FindProperty(k_Opaque);
            if (m_FaceLocked == null) m_FaceLocked = serializedObject.FindProperty(k_FaceLocked);
            if (m_FlipY == null) m_FlipY = serializedObject.FindProperty(k_FlipY);
            if (m_OcclusionMesh == null) m_OcclusionMesh = serializedObject.FindProperty(k_OcclusionMesh);
            if (m_SessionPriority == null) m_SessionPriority = serializedObject.FindProperty(k_SessionPriority);
            if (m_SubmitDepth == null) m_SubmitDepth = serializedObject.FindProperty(k_SubmitDepth);
            if (m_DepthSorting == null) m_DepthSorting = serializedObject.FindProperty(k_DepthSorting);
            if (m_DepthTestRange == null) m_DepthTestRange = serializedObject.FindProperty(k_DepthTestRange);
            if (m_DepthTestNearZ == null) m_DepthTestNearZ = serializedObject.FindProperty(k_DepthTestNearZ);
            if (m_DepthTestFarZ == null) m_DepthTestFarZ = serializedObject.FindProperty(k_DepthTestFarZ);

            serializedObject.Update();

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Varjo settings can't be changed when the editor is in play mode. Use the functions in Varjo.XR.VarjoRendering to modify the settings in runtime.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (selectedBuildTargetGroup == BuildTargetGroup.Standalone)
            {
                EditorGUILayout.PropertyField(m_StereoRenderingMode, s_StereoRenderingModeLabel);
                EditorGUILayout.PropertyField(m_SeparateCullPass, s_SeparateCullPassLabel);
                EditorGUILayout.PropertyField(m_FoveatedRendering, s_FoveatedRenderingLabel);
                EditorGUILayout.PropertyField(m_ContextScalingFactor, s_ContextScalingFactorLabel);
                EditorGUILayout.PropertyField(m_FocusScalingFactor, s_FocusScalingFactorLabel);
                EditorGUILayout.PropertyField(m_Opaque, s_OpaqueLabel);
                EditorGUILayout.PropertyField(m_FaceLocked, s_FaceLockedLabel);
                EditorGUILayout.PropertyField(m_FlipY, s_FlipYLabel);
                EditorGUILayout.PropertyField(m_OcclusionMesh, s_OcclusionMeshLabel);
                EditorGUILayout.PropertyField(m_SessionPriority, s_SessionPriorityLabel);
                EditorGUILayout.PropertyField(m_SubmitDepth, s_SubmitDepthLabel);
                EditorGUILayout.PropertyField(m_DepthSorting, s_DepthSortingLabel);
                EditorGUILayout.PropertyField(m_DepthTestRange, s_DepthTestRangeLabel);
                EditorGUILayout.Slider(m_DepthTestNearZ, 0f, m_DepthTestFarZ.floatValue - 0.00001f);
                EditorGUILayout.Slider(m_DepthTestFarZ, m_DepthTestNearZ.floatValue + 0.00001f, 50f);
            }
            else
            {
                EditorGUILayout.HelpBox("Varjo XR plugin is not supported for this target platform.", MessageType.Info);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
