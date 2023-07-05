using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    public static RecruitManager instance;

    [SerializeField] private List<ObjectPool> recruitPools = new();
    private readonly List<GameObject> activeRecruits = new();
    GameObject player;


    [Header("Base Spawning Attributes")]
    [SerializeField] private int maxSpawnDistance;
    [SerializeField] private int initialAmountToSpawn;

    [Header(" Timed Spawning")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private int amountToSpawnPerInterval;


    [Header(" Threshold Spawning")]
    [SerializeField] private bool EnableThresholdSpawning = false;
    [SerializeField] private int enemyKillThreshold;
    [SerializeField] private int thresholdMultiplier;
    [SerializeField] private int amountToSpawn;


    private int killCount = 0;


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

    private void OnDestroy()
    {
        instance = null;
    }

    // Start is called before the first frame update
    private void Start()
    {
        SpawnRecruitBatch(initialAmountToSpawn);
        StartCoroutine(SpawnCoroutine());


    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(amountToSpawnPerInterval);

            SpawnRecruitBatch(amountToSpawnPerInterval);
        }
    }

    private void SpawnRecruitBatch(int amount)
    {
        player = GameManager.instance.Player;
        Debug.Log(player.transform.position);
        for (int i = 0; i < amount; i++)
        {
            // spawn point around the player
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 spawnPoint = player.transform.position + (dir * Random.Range(20f, maxSpawnDistance));

            //if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
            //{
            //    spawnPoint = spawnPoint.normalized * maxSpawnDistance;
            //}
            
            PlayerUnitType toSpawn = PlayerUnitType.Neutrophil;
            float rand = Random.value;

            if (rand < 0.1f)
            {
                toSpawn = PlayerUnitType.Dendritic;
            }
            else if (rand < 0.3f)
            {
                toSpawn = PlayerUnitType.Macrophage;
            }
            else if (rand < 0.7f)
            {
                toSpawn = PlayerUnitType.Neutrophil;
            }

            SpawnRecruit(spawnPoint, toSpawn);
        }
    }

    public void SpawnRecruit(Vector3 position, PlayerUnitType type)
    {
        GameObject recruit = recruitPools[(int)type].RequestPoolable(position);

        if (!recruit)
        {
            Debug.LogWarning("No recruit found in object pool!");
            return;
        }

        activeRecruits.Add(recruit);
    }

    public void AddKillCount()
    {
        if (!EnableThresholdSpawning)
        {
            return;
        }
        killCount++;

        if (killCount >= enemyKillThreshold)
        {
            // spawn recruit
            SpawnRecruitBatch(amountToSpawn);

            killCount = 0;
            // increase kill threshold

            if (thresholdMultiplier != 0)
            {
                enemyKillThreshold *= thresholdMultiplier;
            }
        }
    }
}
