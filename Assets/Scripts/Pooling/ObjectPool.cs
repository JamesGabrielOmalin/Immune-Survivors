using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pooling")]
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int amountToPool;

    public readonly List<GameObject> objectPool = new();

    private void Awake()
    {
        PoolObjects();
    }

    private async void PoolObjects()
    {
        var tasks = new Task[amountToPool];
        for (int i = 0; i < amountToPool; i++)
        {
            tasks[i] = PoolObject();
        }

        await Task.WhenAll(tasks);
    }

    private async Task PoolObject()
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
            return null;

        obj.transform.position = position;
        obj.SetActive(true);
    
        return obj;
    }

}
