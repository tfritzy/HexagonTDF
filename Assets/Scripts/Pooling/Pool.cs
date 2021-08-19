using UnityEngine;
using System.Collections.Generic;

public class Pool
{
    private Dictionary<int, LinkedList<GameObject>> _objects;
    private Dictionary<int, GameObject> prefabs;

    public Pool(Dictionary<int, GameObject> prefabs)
    {
        this.prefabs = prefabs;
    }

    public GameObject GetObject(int objectType)
    {
        if (_objects == null)
        {
            _objects = new Dictionary<int, LinkedList<GameObject>>();
        }

        if (_objects.ContainsKey(objectType) == false)
        {
            _objects[objectType] = new LinkedList<GameObject>();
        }

        if (_objects[objectType].Count > 0)
        {
            GameObject poolObject = _objects[objectType].First.Value;
            poolObject.SetActive(true);
            poolObject.GetComponent<PoolObject>().Setup(objectType, this);
            _objects[objectType].RemoveFirst();
            return poolObject;
        }
        else
        {
            GameObject newPoolObject = this.CreateObject(objectType);
            newPoolObject.name = objectType + "_" + System.Guid.NewGuid().ToString("N");
            newPoolObject.AddComponent<PoolObject>().Setup(objectType, this);
            return newPoolObject;
        }
    }

    public void ReturnObject(GameObject poolObject, int objectType)
    {
        ResetPoolObject(poolObject);
        _objects[objectType].AddLast(poolObject);
        poolObject.SetActive(false);
    }

    private void ResetPoolObject(GameObject poolObject)
    {
        if (poolObject.GetComponent<Collider>() != null)
        {
            poolObject.GetComponent<Collider>().enabled = true;
        }
    }

    private GameObject CreateObject(int objectType)
    {
        return GameObject.Instantiate(prefabs[objectType]);
    }
}