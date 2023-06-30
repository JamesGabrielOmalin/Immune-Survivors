using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    public static RecruitManager instance;

    [SerializeField] private List<ObjectPool> recruitPools = new();
    private readonly List<GameObject> activeRecruits = new();
    
    private enum SpawningType
    {
        Timed_Spawning,
        Threshold_Spawning
    }
    [Header("Base Spawning Attributes")]
    [SerializeField] private int maxSpawnDistance;
    [SerializeField] private int initialAmountToSpawn;

    [SerializeField] private int amountToSpawn;

    [SerializeField] SpawningType spawnType = SpawningType.Timed_Spawning;

    [Header(" Timed Spawning")]
    [SerializeField] private float spawnInterval;

    [Header(" Threshold Spawning")]

    [SerializeField] private int enemyKillThreshold;

    [SerializeField] private int thresholdMultiplier;

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

        switch (spawnType)
        {
            case SpawningType.Timed_Spawning:
                Debug.Log("[Recruit Manager] :  Batch Spawning");
                StartCoroutine(SpawnCoroutine());
                break;

            case SpawningType.Threshold_Spawning:
                Debug.Log("[Recruit Manager] :  Threshold Spawning");
                break;
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(amountToSpawn);

            SpawnRecruitBatch(amountToSpawn);
        }
    }

    private void SpawnRecruitBatch(int amount)
    {
        GameObject player = GameManager.instance.Player;
        for (int i = 0; i < amount; i++)
        {
            // spawn point around the player
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 spawnPoint = player.transform.position + dir * Random.Range(20f, 40f);

            if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
            {
                spawnPoint = spawnPoint.normalized * maxSpawnDistance;
            }
            
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
        killCount++;

        if (killCount >= enemyKillThreshold)
        {
            // spawn recruit
            SpawnRecruitBatch(amountToSpawn);

            killCount = 0;
            // increase kill threshold
            enemyKillThreshold *= thresholdMultiplier;
        }
    }
}
