using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [SerializeField] private List<ObjectPool> enemyPools = new();
    public List<GameObject> activeEnemies = new();

    public int InfectionRate => activeEnemies.FindAll(enemy => enemy.activeInHierarchy).Count;
    [field: Header("Infection")]
    [field: SerializeField]
    public int MinInfectionRate { get; private set; }
    [field: SerializeField]
    public int MaxInfectionRate { get; private set; }


    //[field: SerializeField]
    //public int InitialAmountToSpawn { get; private set; }
    //[field: SerializeField]
    //public int AmountPerBatch { get; private set; }
    //[field: SerializeField]
    //public float BatchSpawnInterval { get; private set; }

    [SerializeField] private float maxSpawnDistance;

    [field: Header("Wave")]
    [SerializeField] LevelWaveData levelWaveData;

    [SerializeField] private int WaveNumber = 0;
    [SerializeField] private Wave currentWave;
    [SerializeField] private int waveInterval = 0;
    [SerializeField] private int maxEnemyCountAllowed;


    public System.Action OnMinInfectionReached;
    public System.Action OnMaxInfectionReached;
    public System.Action OnInfectionRateChanged;

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

    // Start is called before the first frame update
    private void Start()
    {
        // Set the current wave with the firt wave in the wavelist
        currentWave = levelWaveData.waveList[WaveNumber];

        // Calculate the quota of the current wave
        CalculateWaveQuota();

        // Start couroutine for wave and spawning
        StartCoroutine(WaveCoroutine());
        StartCoroutine(SpawnCoroutine());
    }

    private void OnDestroy()
    {
        instance = null;
    }
    private IEnumerator WaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);

            // Increment wave number if there is a next wave
            if (WaveNumber < levelWaveData.waveList.Count-1)
            {
                WaveNumber++;
                currentWave = levelWaveData.waveList[WaveNumber];
                currentWave.spawnCounter = 0;
                foreach(EnemyGroup eg in currentWave.enemyGroups)
                {
                    eg.spawnCounter = 0;
                }

                CalculateWaveQuota();

            }
            else
            {
                Debug.Log(" Last Wave Ended");
            }

        }

    }
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentWave.spawnInterval);

            int totalEnemyCount = EnemyManager.instance.activeEnemies.Count;

           
            // Spawn enemy of each type if the number of enemies present in below the wave quota
            if (currentWave.spawnCounter < currentWave.waveQuota && totalEnemyCount < maxEnemyCountAllowed)
            {
                // Spawn each type of enemy
                foreach(EnemyGroup eg in currentWave.enemyGroups)
                {
                    // Spawn until quota is reached
                    if(eg.spawnCounter < eg.enemyQuota)
                    {
                        SpawnEnemyBatch(1, eg.antigenType);
                        eg.spawnCounter++;
                        currentWave.spawnCounter++;
                    }
                }
            }
            // Spawn each type of enemy if the enemies present are more than the wave quota
            else if (currentWave.spawnCounter >= currentWave.waveQuota && totalEnemyCount < maxEnemyCountAllowed)
            {
                int type = Random.Range(0, currentWave.enemyGroups.Count);

                // spawn this type
                SpawnEnemyBatch(1, currentWave.enemyGroups[type].antigenType);
                currentWave.enemyGroups[type].spawnCounter++;
                currentWave.spawnCounter++;

            }
        }
    }
    private void SpawnEnemyBatch(int amount, int antigenType)
    {
        GameObject player = GameManager.instance.Player;
        for (int i = 0; i < amount; i++)
        {
            // spawn point around the player
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 spawnPoint = player.transform.position + dir * Random.Range(30f, 40f);

            // Limit spawn position
            if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
            {
                spawnPoint = spawnPoint.normalized * maxSpawnDistance;
            }

            //float x = Random.Range(-50f, 50f);
            //float z = Random.Range(-50f, 50f);

            SpawnEnemy(spawnPoint, (AntigenType)antigenType);
        }
    }

    public void SpawnEnemy(Vector3 position, AntigenType type = AntigenType.Type_1)
    {
        GameObject enemy = enemyPools[(int)type].RequestPoolable(position);
        
        if (!enemy)
        {
            Debug.LogWarning("No enemy found in object pool!");
            return;
        }

        if (enemy.TryGetComponent<Collider>(out Collider cc))
        {
            cc.enabled = true;
        }


        //Debug.Log(position);

        //enemy.transform.position = position;

        activeEnemies.Add(enemy);
        //InfectionRate++;
        OnInfectionRateChanged?.Invoke();

        enemy.GetComponent<Enemy>().OnDeath += delegate 
        { 
            activeEnemies.Remove(enemy);
            //InfectionRate--;
            OnInfectionRateChanged?.Invoke();

            // Win Condition
            if (InfectionRate <= MinInfectionRate)
            {
                OnMinInfectionReached?.Invoke();
            }
        };

        // Lose Condition
        if (InfectionRate >= MaxInfectionRate)
        {
            OnMaxInfectionReached?.Invoke();
        }
    }

    private void CalculateWaveQuota()
    {
        
        int currentWaveQuota = 0;

        // Current wave quota  = accumulative sum of the enemy groups in the current wave
        foreach (EnemyGroup eg in currentWave.enemyGroups)
        {
            currentWaveQuota += eg.enemyQuota;
        }

        currentWave.waveQuota = currentWaveQuota;
        Debug.Log("Wave " + currentWave.WaveID + " Quota:" + currentWaveQuota);
    }
    public GameObject GetNearestEnemy(Vector3 position, float limit = float.MaxValue)
    {
        if (activeEnemies.Count < 1)
        {
            return null;
        }

        float distance;
        
        GameObject nearest = null;
        distance = limit;

        foreach (var unit in activeEnemies)
        {
            bool isDead = unit.GetComponent<Enemy>().IsDead;
            // Skip if unit is inactive or is dead
            if (!unit.activeInHierarchy || isDead)
                continue;

            float dist = Vector3.Distance(unit.transform.position, position);
            if (dist < distance)
            {
                nearest = unit;
                distance = dist;
            }
        }

        return nearest;
    }

    public GameObject GetFurthestEnemy(Vector3 position, float limit = float.MaxValue)
    {
        if (activeEnemies.Count < 1)
        {
            return null;
        }

        float distance;

        GameObject furthest = null;
        distance = 0f;

        foreach (var unit in activeEnemies)
        {
            bool isDead = unit.GetComponent<Enemy>().IsDead;
            // Skip if unit is inactive or is dead
            if (!unit.activeInHierarchy || isDead)
                continue;

            float dist = Vector3.Distance(unit.transform.position, position);
            if (dist > distance && dist <= limit)
            {
                furthest = unit;
                distance = dist;
            }
        }

        return furthest;
    }
}
