using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pooling")]
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int amountToPool;
    [SerializeField] private bool addIfNotEnough;

    public readonly List<GameObject> objectPool = new();

    private void Awake()
    {
        PoolObjects();
    }

    private void PoolObjects()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            PoolObject();
        }
    }

    private GameObject PoolObject()
    {
        GameObject instance = Instantiate(objectToPool, transform);
        instance.SetActive(false);
        objectPool.Add(instance);

        return instance;
    }

    private async void PoolObjects_Async()
    {
        var tasks = new Task[amountToPool];
        for (int i = 0; i < amountToPool; i++)
        {
            tasks[i] = PoolObjectAsync();
        }

        await Task.WhenAll(tasks);
    }

    private async Task PoolObjectAsync()
    {
        GameObject instance = Instantiate(objectToPool, transform);
        instance.SetActive(false);
        objectPool.Add(instance);

        await Task.Yield();
    }

    public GameObject RequestPoolable(Vector3 position)
    {
        GameObject obj = objectPool.Find(obj => !obj.activeInHierarchy);
        if (obj == null)
        {
            if (!addIfNotEnough)
                return null;
            obj = PoolObject();
        }

        obj.transform.position = position;
        obj.SetActive(true);
    
        return obj;
    }

}
