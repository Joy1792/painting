using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private const int MAX_POOL_CAPACITY = 50;

    [SerializeField]
    private Transform pooledObjectParent;

    private Stack<GameObject> objectPool = new Stack<GameObject>(MAX_POOL_CAPACITY);

    private static Pool instance = null;
    public static Pool Instance { get => instance; }

    [SerializeField]
    private GameObject pooledObjectPrefab;
    private void Awake()
    {
        if (instance == null) instance = this;

    }

    private void Start()
    {
        for (int i = 0; i < MAX_POOL_CAPACITY; i++)
        {
            CreateObject( pooledObjectParent);
        }
    }

    public GameObject CreateObject(Transform parent)
    {
        GameObject go;
        if (parent == null)
             go = Instantiate(pooledObjectPrefab);
        else 
            go = Instantiate(pooledObjectPrefab,parent);

        go.SetActive(false);
        objectPool.Push(go);
        return go;
    }
    public bool IsEmpty()
    {
        return (objectPool.Count == 0);
    }

    public void AddObject(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Push(obj);
    }

    public GameObject GetObject()
    {
        if (IsEmpty())
        {
            CreateObject(pooledObjectParent);
        }
        GameObject go = objectPool.Pop();
        go.SetActive(true);
        return go;

    }
}
