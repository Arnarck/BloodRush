using UnityEngine;
using System.Collections.Generic;

public class PoolingSystem : MonoBehaviour
{
    [Tooltip("The prefab GameObject that the pool will instantiate.")]
    [SerializeField] GameObject objectPrefab;

    [Tooltip("The initial amount of GameObjects that will be instantiated at the start of the game.")]
    [SerializeField] int poolSize = 20;

    List<GameObject> _pool;

    public GameObject PoolObject { get => objectPrefab; }

    void Awake()
    {
        PopulatePool();
    }

    void PopulatePool()
    {
        _pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolObject = GeneratePoolObject();
            poolObject.gameObject.SetActive(false);
        }
    }

    // Instantiates a GameObject, add it to the pool and set its parent.
    GameObject GeneratePoolObject()
    {
        GameObject poolObject = Instantiate(objectPrefab);
       
        _pool.Add(poolObject);
        poolObject.transform.SetParent(transform);
        return poolObject;
    }

    // Activates the first disabled object found. Instantiates another object if no disabled object is found.
    public GameObject GetObject()
    {
        foreach (GameObject item in _pool)
        {
            if (!item.activeInHierarchy)
            {
                item.gameObject.SetActive(true);
                return item;
            }
        }

        GameObject poolObject = GeneratePoolObject();
        return poolObject;
    }
}
