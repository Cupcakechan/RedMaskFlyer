using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private readonly List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
            CreateInstance();
    }

    private GameObject CreateInstance()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public GameObject Get()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject grown = CreateInstance();   // pool ran dry — grow it
        grown.SetActive(true);
        return grown;
    }
}