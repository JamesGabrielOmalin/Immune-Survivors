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
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void SpawnAdaptiveCell(AntigenType type)
    {
        //StartCoroutine(SpawnCoroutine(type));
        GameObject player = GameManager.instance.Player;
        // spawn point around the player
        float angle = Random.Range(0f, 360f);
        Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        Vector3 spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

        if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
        {
            spawnPoint = spawnPoint.normalized * maxSpawnDistance;
        }

        GameObject helperTCell = helperTCellPool.RequestPoolable(spawnPoint);
        helperTCell.GetComponent<AdaptiveCell>().SetType(type);

        angle = Random.Range(0f, 360f);
        dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

        if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
        {
            spawnPoint = spawnPoint.normalized * maxSpawnDistance;
        }

        GameObject bCell = BCellPool.RequestPoolable(spawnPoint);
        bCell.GetComponent<AdaptiveCell>().SetType(type);
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
}
