using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintable : MonoBehaviour
{

    [SerializeField]
    private MeshFilter meshFilter;

     [SerializeField]
    private MeshCollider meshCollider;

    public MeshCollider MeshCollider { get => meshCollider; set => meshCollider = value; }

    private void Awake()
    {
        meshCollider.sharedMesh = meshFilter.sharedMesh;

        meshCollider.transform.localScale = Vector3.one;
        meshCollider.gameObject.layer = LayerMask.NameToLayer("Paintable");
    }

    
}
