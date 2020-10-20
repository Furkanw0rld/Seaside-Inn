using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillerAgent : MonoBehaviour, ISkinnedMeshReference
{
#pragma warning disable 0649
    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;
#pragma warning restore 0649

    public SkinnedMeshRenderer MeshRenderer { get { return meshRenderer; } }
}
