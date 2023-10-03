using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveManager : MonoBehaviour
{
    [SerializeField] private ObjectPool helperTCellPool;
    [SerializeField] private ObjectPool BCellPool;
    [SerializeField] public ObjectPool cytokinePool;
    [SerializeField] public ObjectPool antibodyPool;

    [SerializeField] private float maxSpawnDistance;

    [SerializeField] private float spawnInterval;

    [SerializeField] private BoxCollider spawnArea;

    public static AdaptiveManager instance; 

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        AntigenManager.instance.OnAntigenThresholdReached[AntigenType.Type_1] += (() => SpawnAdaptiveCell(AntigenType.Type_1));
        AntigenManager.instance.OnAntigenThresholdReached[AntigenType.Type_2] += (() => SpawnAdaptiveCell(AntigenType.Type_2));
        AntigenManager.instance.OnAntigenThresholdReached[AntigenType.Type_3] += (() => SpawnAdaptiveCell(AntigenType.Type_3));
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void SpawnAdaptiveCell(AntigenType type)
    {
        StartCoroutine(SpawnCoroutine(type));
        GameObject player = GameManager.instance.Player;
        
        
        // spawn point around the player
        //float angle = Random.Range(0f, 360f);
        //Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        //Vector3 spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

        //if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
        //{
        //    spawnPoint = spawnPoint.normalized * maxSpawnDistance;
        //}

        //GameObject helperTCell = helperTCellPool.RequestPoolable(spawnPoint);
        //helperTCell.GetComponent<AdaptiveCell>().SetType(type);

        //angle = Random.Range(0f, 360f);
        //dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        //spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

        //if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
        //{
        //    spawnPoint = spawnPoint.normalized * maxSpawnDistance;
        //}

        //GameObject bCell = BCellPool.RequestPoolable(spawnPoint);
        //bCell.GetComponent<AdaptiveCell>().SetType(type);
    }

    //private IEnumerator SpawnCoroutine(AntigenType type)
    //{
    //    GameObject player = GameManager.instance.Player;
    //    WaitForSeconds wait = new(10f);

    //    while (this)
    //    {
    //        // spawn point around the player
    //        float angle = Random.Range(0f, 360f);
    //        Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
    //        Vector3 spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

    //        if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
    //        {
    //            spawnPoint = spawnPoint.normalized * maxSpawnDistance;
    //        }

    //        GameObject helperTCell = helperTCellPool.RequestPoolable(spawnPoint);
    //        helperTCell.GetComponent<AdaptiveCell>().SetType(type);

    //        angle = Random.Range(0f, 360f);
    //        dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
    //        spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

    //        if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
    //        {
    //            spawnPoint = spawnPoint.normalized * maxSpawnDistance;
    //        }

    //        GameObject bCell = BCellPool.RequestPoolable(spawnPoint);
    //        bCell.GetComponent<AdaptiveCell>().SetType(type);

    //        yield return wait;
    //    }
    //}

    private IEnumerator SpawnCoroutine(AntigenType type)
    { 
        bool canSpawn = false;
        Vector3 spawnPoint = Vector3.zero;

        do
        {
            yield return new WaitForSeconds(1f);
            spawnPoint = RandomPointInBounds(spawnArea.bounds);
            spawnPoint.y = 0;


            bool isColliding = Physics.CheckSphere(spawnPoint, 2, LayerMask.NameToLayer("Adaptive"));

            if (isColliding)
            {
                Debug.Log("Spheres are colliding!");
                // You can add your collision logic here
            }
            else
            {
                canSpawn= true;
            }
        } while (canSpawn == false);

        GameObject helperTCell = helperTCellPool.RequestPoolable(spawnPoint);
        helperTCell.GetComponent<AdaptiveCell>().SetType(type);

        GameObject bCell = BCellPool.RequestPoolable(spawnPoint);
        bCell.GetComponent<AdaptiveCell>().SetType(type);
    }

    private Vector3 RandomPointInBounds(in Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
