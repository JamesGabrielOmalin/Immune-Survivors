using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    public static RecruitManager instance;

    [SerializeField] private List<ObjectPool> recruitPools = new();
    private readonly List<GameObject> activeRecruits = new();
    
    [Header("Spawning")]
    [SerializeField] private int initialAmountToSpawn;
    [SerializeField] private float batchSpawnInterval;
    [SerializeField] private int amountPerBatch;
    [SerializeField] private int maxSpawnDistance;

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
            yield return new WaitForSeconds(batchSpawnInterval);

            SpawnRecruitBatch(amountPerBatch);
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
}
