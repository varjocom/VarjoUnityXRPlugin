#if VARJO_EXPERIMENTAL_FEATURES

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Varjo.XR;

public class VarjoReconstruction : MonoBehaviour
{
    public Transform meshParent;
    public GameObject meshPrefab;
    public int meshQueueSize = 50;
    public float meshInfoUpdateInterval = 0.1f;

    public Dictionary<MeshId, GameObject> meshIdToGameObjectMap;

    VarjoLoader m_Loader;
    XRMeshSubsystem m_MeshSubsystem;
    List<MeshInfo> m_MeshInfos;
    Dictionary<MeshId, MeshInfo> m_MeshesNeedingGeneration;
    Dictionary<MeshId, MeshInfo> m_MeshesBeingGenerated;
    bool m_initialized = false;
    float m_lastMeshInfoUpdateTime;

    void Awake()
    {
        m_MeshInfos = new List<MeshInfo>();
        m_MeshesNeedingGeneration = new Dictionary<MeshId, MeshInfo>();
        m_MeshesBeingGenerated = new Dictionary<MeshId, MeshInfo>();
        meshIdToGameObjectMap = new Dictionary<MeshId, GameObject>();
    }

    IEnumerator Initialize()
    {
        while (XRGeneralSettings.Instance == null)
        {
            yield return null;
        }

        while (XRGeneralSettings.Instance.Manager == null)
        {
            yield return null;
        }

        m_Loader = XRGeneralSettings.Instance.Manager.ActiveLoaderAs<VarjoLoader>();

        if (m_Loader != null)
            m_MeshSubsystem = m_Loader.meshSubsystem;

        StartSubsystem();

        m_initialized = true;
    }

    void StartSubsystem()
    {
        if (m_Loader != null)
            m_Loader.StartMeshSubsystem();
    }

    void StopSubsystem()
    {
        m_initialized = false;
        if (m_Loader != null)
            m_Loader.StopMeshSubsystem();
        m_MeshSubsystem = null;
    }

    void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    void OnDisable()
    {
        StopSubsystem();
    }

    void Update()
    {
        if (m_MeshSubsystem == null)
            return;

        if (!m_MeshSubsystem.running)
            return;

        if (m_initialized)
        {
            if (Time.time - m_lastMeshInfoUpdateTime < meshInfoUpdateInterval)
                return;

            if (m_MeshSubsystem.TryGetMeshInfos(m_MeshInfos))
            {
                foreach (var meshInfo in m_MeshInfos)
                {
                    switch (meshInfo.ChangeState)
                    {
                        case MeshChangeState.Added:
                        case MeshChangeState.Updated:
                            m_MeshesNeedingGeneration[meshInfo.MeshId] = meshInfo;
                            break;

                        case MeshChangeState.Removed:
                            m_MeshesNeedingGeneration.Remove(meshInfo.MeshId);

                            GameObject meshGameObject;
                            if (meshIdToGameObjectMap.TryGetValue(meshInfo.MeshId, out meshGameObject))
                            {
                                Destroy(meshGameObject);
                                meshIdToGameObjectMap.Remove(meshInfo.MeshId);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            m_lastMeshInfoUpdateTime = Time.time;
        }

        while (m_MeshesBeingGenerated.Count < meshQueueSize && m_MeshesNeedingGeneration.Count > 0)
        {
            MeshId meshId;

            if (GetNextMeshToGenerate(out meshId))
            {
                var meshGameObject = GetOrCreateGameObjectForMesh(meshId);
                var meshFilter = meshGameObject.GetComponent<MeshFilter>();
                var mesh = GetOrCreateMesh(meshFilter);

                var meshAttributes = (MeshVertexAttributes.Normals | MeshVertexAttributes.Colors | MeshVertexAttributes.UVs);

                m_MeshSubsystem.GenerateMeshAsync(meshId, mesh, null, meshAttributes, OnMeshGenerated);

                m_MeshesBeingGenerated.Add(meshId, m_MeshesNeedingGeneration[meshId]);
                m_MeshesNeedingGeneration.Remove(meshId);
            }
        }
    }

    Mesh GetOrCreateMesh(MeshFilter meshFilter)
    {
        if (meshFilter == null)
            return null;

        if (meshFilter.sharedMesh != null)
            return meshFilter.sharedMesh;

        return meshFilter.mesh;
    }

    void OnMeshGenerated(MeshGenerationResult result)
    {
        if (result.Status != MeshGenerationStatus.Success)
        {
            GameObject meshGameObject;
            if (meshIdToGameObjectMap.TryGetValue(result.MeshId, out meshGameObject))
            {
                Destroy(meshGameObject);
                meshIdToGameObjectMap.Remove(result.MeshId);
            }
        }
        m_MeshesBeingGenerated.Remove(result.MeshId);
    }

    bool GetNextMeshToGenerate(out MeshId meshId)
    {
        Nullable<MeshInfo> highestPriorityMeshInfo = null;

        foreach (var meshInfo in m_MeshesNeedingGeneration.Values)
        {
            if (!highestPriorityMeshInfo.HasValue)
            {
                highestPriorityMeshInfo = meshInfo;
                continue;
            }

            if (meshInfo.PriorityHint > highestPriorityMeshInfo.Value.PriorityHint)
            {
                highestPriorityMeshInfo = meshInfo;
            }
        }

        if (highestPriorityMeshInfo.HasValue)
        {
            meshId = highestPriorityMeshInfo.Value.MeshId;
            return true;
        }
        else
        {
            meshId = MeshId.InvalidId;
            return false;
        }
    }

    GameObject GetOrCreateGameObjectForMesh(MeshId meshId)
    {
        GameObject meshGameObject;
        if (!meshIdToGameObjectMap.TryGetValue(meshId, out meshGameObject))
        {
            meshGameObject = Instantiate(meshPrefab, meshParent);
            meshGameObject.name = meshId.ToString();
            meshIdToGameObjectMap.Add(meshId, meshGameObject);
        }
        return meshGameObject;
    }

    public void Reset(bool destroyGameObjects = true)
    {
        bool wasRunning = m_initialized;

        StopSubsystem();

        if (destroyGameObjects)
        {
            foreach (var meshGameObject in meshIdToGameObjectMap.Values)
            {
                Destroy(meshGameObject);
            }
        }
        m_MeshInfos.Clear();
        meshIdToGameObjectMap.Clear();
        m_MeshesNeedingGeneration.Clear();
        m_MeshesBeingGenerated.Clear();

        if (wasRunning)
        {
            StartCoroutine(Initialize());
        }
    }

    public void SetBoundingVolume(Vector3 origin, Vector3 extents)
    {
        if (m_MeshSubsystem == null) return;
        m_MeshSubsystem.SetBoundingVolume(origin, extents);
    }

    public void ResetBoundingVolume()
    {
        if (m_MeshSubsystem == null) return;
        m_MeshSubsystem.SetBoundingVolume(Vector3.zero, Vector3.zero);
    }

    public bool IsRunning()
    {
        if (m_MeshSubsystem == null)
            return false;

        return m_initialized && m_MeshSubsystem.running;
    }
}

#endif