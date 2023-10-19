using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AdaptiveManager : MonoBehaviour
{
    [Header("Object Pools")]
    [SerializeField] private ObjectPool helperTCellPool;
    [SerializeField] private ObjectPool BCellPool;
    [SerializeField] public ObjectPool cytokinePool;
    [SerializeField] public ObjectPool antibodyPool;

    [Header("Basic Spawning Attributes")]

    //[SerializeField] private float maxSpawnDistance;
    //[SerializeField] private float spawnInterval;
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private LayerMask layersToCheck;
    [SerializeField] private float collisionCheckerRadius;
    [SerializeField] private int maxChecks;


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
        spawnArea = GameObject.FindGameObjectWithTag("SpawnArea").GetComponent<BoxCollider>();
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
        StartSpawnCoroutines(type);
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

    private IEnumerator StartSpawnCoroutines(AntigenType type)
    {
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SpawnTCellCoroutine(type));
        StartCoroutine(SpawnBCellCoroutine(type));

    }

    private IEnumerator SpawnTCellCoroutine(AntigenType type)
    {
        yield return new WaitForSeconds(0.25f);

        bool canSpawn = false;
        int nChecks = 0;

        Vector3 spawnPoint = RandomPointInBounds(spawnArea.bounds);
        spawnPoint.y = 0;

        do
        {
            bool isColliding = Physics.CheckSphere(spawnPoint, collisionCheckerRadius, layersToCheck);

            if (isColliding)
            {
                //Debug.Log("has Collision");
                spawnPoint = RandomPointInBounds(spawnArea.bounds);
                spawnPoint.y = 0;
                nChecks++;
            }
            else
            {
                canSpawn = true;
            }
            //Debug.Log(isColliding);

        } while (!canSpawn && nChecks < maxChecks);

        GameObject helperTCell = helperTCellPool.RequestPoolable(spawnPoint);
        helperTCell.GetComponent<AdaptiveCell>().SetType(type);
    }

    private IEnumerator SpawnBCellCoroutine(AntigenType type)
    {
        yield return new WaitForSeconds(0.25f);

        bool canSpawn = false;
        int nChecks = 0;

        Vector3 spawnPoint = RandomPointInBounds(spawnArea.bounds);
        spawnPoint.y = 0;

        do
        {
            bool isColliding = Physics.CheckSphere(spawnPoint, collisionCheckerRadius, layersToCheck);

            if (isColliding)
            {
                //Debug.Log("has Collision");
                spawnPoint = RandomPointInBounds(spawnArea.bounds);
                spawnPoint.y = 0;
                nChecks++;
            }
            else
            {
                canSpawn = true;
            }
            //Debug.Log(isColliding);

        } while (!canSpawn && nChecks < maxChecks);

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
