using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Tooltip("Variant prefabs for this pool. Get() returns a random one.")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int initialSizePerPrefab = 5;

    private readonly List<List<GameObject>> subPools = new List<List<GameObject>>();

    void Awake()
    {
        if (prefabs == null) return;

        foreach (GameObject prefab in prefabs)
        {
            var list = new List<GameObject>();
            for (int i = 0; i < initialSizePerPrefab; i++)
                list.Add(CreateInstance(prefab));
            subPools.Add(list);
        }
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get()
    {
        if (prefabs == null || prefabs.Length == 0) return null;

        int variant = Random.Range(0, prefabs.Length);
        List<GameObject> list = subPools[variant];

        foreach (GameObject obj in list)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject grown = CreateInstance(prefabs[variant]);   // grow this variant
        list.Add(grown);
        grown.SetActive(true);
        return grown;
    }
}