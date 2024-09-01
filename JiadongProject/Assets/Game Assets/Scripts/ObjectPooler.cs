using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public Transform parent;
    public GameObject[] prefabs;
    public int initialPoolSize = 5;
    public int poolIncrement = 3;

    [HorizontalLine]
    
    [ReadOnly] public List<GameObject> pool;
    [ReadOnly] public List<GameObject> allObjects;

    void Start()
    {
        pool = new List<GameObject>();

        PopulatePool(initialPoolSize);
    }

    void Update()
    {
        allObjects = allObjects.Where(item => item != null).ToList();
        pool.RemoveAll(item => item == null);
        allObjects.RemoveAll(item => item == null);
    }

    GameObject InstantiateObj()
    {
        if (prefabs.Length == 0) return null;
        GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)], parent);
        obj.SetActive(false);
        return obj;
    }

    void PopulatePool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newObj = InstantiateObj();
            pool.Add(newObj);
            allObjects.Add(newObj);
        }
    }

    public void ResetActiveObjectsToPool()
    {
        foreach (var obj in allObjects)
        {
            if (obj.activeSelf)
            {
                InsertToPool(obj);
            }
        }
    }

    public GameObject GrabFromPool(Vector3 position, Quaternion rotation)
    {
        // Check if the pool is empty
        if (pool.Count == 0)
            PopulatePool(poolIncrement);

        GameObject obj = GrabFromBottom();
        if(obj == null) return null;

        obj.SetActive(true);

        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public void InsertToPool(GameObject obj)
    {
        obj.SetActive(false);
        InsertAtTop(obj);
    }

    void InsertAtTop(GameObject gameObject)
    {
        // Insert the gameObject at the beginning of the list
        pool.Insert(0, gameObject);
    }

    GameObject GrabFromBottom()
    {
        // Check if the list is empty
        if (pool.Count == 0)
        {
            return null;
        }

        // Get the gameObject from the bottom of the list
        GameObject gameObject = pool[pool.Count - 1];

        // Remove the gameObject from the list
        pool.RemoveAt(pool.Count - 1);

        return gameObject;
    }

    public int CountActiveObjects()
    {
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj != null && obj.activeSelf) count++;
        }
        return count;
    }
}
