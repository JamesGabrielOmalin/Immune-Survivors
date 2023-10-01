using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [SerializeField] private List<EnemyPool> enemyPools = new();
    public List<GameObject> allEnemies = new();
    public List<GameObject> activeEnemies = new();

    public int InfectionRate => activeEnemies.FindAll(enemy => enemy.activeInHierarchy).Count;
    [field: Header("Infection")]
    [field: SerializeField]
    public int MinInfectionRate { get; private set; }
    [field: SerializeField]
    public int MaxInfectionRate { get; private set; }

    [field: SerializeField]
    public int SymptomThreshold { get; private set; }

    [SerializeField] private BoxCollider spawnArea;

    [SerializeField] private float maxSpawnDistance;

    [SerializeField] private GameObject track;


    [field: Header("Wave")]
    [SerializeField] LevelWaveData level;

    Coroutine waveCoroutine;
    Coroutine spawnCoroutine;

    private int lastWaveIndex;
    [SerializeField] private int waveIndex = 0;
    [SerializeField] private Wave currentWave;

    [Header("Viewport Bounds Threshold")]

    [Tooltip("Relocate objects when the x units away from the viewport screen")]
    [SerializeField] private Vector2 boundsThreshold;

    [Tooltip("Relocate objects by x units outside of the viewport screen (must be less than bouds threshold)")]
    [SerializeField] private float spawnThreshold;

    [Tooltip("Within (0-1)  LL Viewing fustrum (0,0) ")]
    private Vector2 minBounds = new Vector2(0, 0);
    [Tooltip("Within (0-1)  UR Viewing fustrum (1,1) ")]
    private Vector2 maxBounds = new Vector2(1, 1);

    private Camera cam;



    public System.Action OnMinInfectionReached;
    public System.Action OnMaxInfectionReached;
    public System.Action OnInfectionRateChanged;

    //public System.Action OnSymptomThresholdNotReached;
    //public System.Action OnSymptomThresholdReached;


    [System.Serializable]
    public class EnemyPool
    {
        public string Name;
        public ObjectPool enemyPool;
    }

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
        cam = Camera.main;

        minBounds -= boundsThreshold;
        maxBounds += boundsThreshold;

        // Set the current wave with the firt wave in the wavelist
        InitalizeCurrentWave(waveIndex);
        lastWaveIndex = level.waveList.Count - 1;

        // Calculate the quota of the current wave
        CalculateWaveQuota();

        // Start couroutine for wave and spawning
        waveCoroutine = StartCoroutine(WaveCoroutine());
        spawnCoroutine = StartCoroutine(BasicEnemySpawnCoroutine());

        //StartCoroutine(RelocateEnemies());

    }

    private void OnDestroy()
    {
        instance = null;
    }
    private IEnumerator WaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(level.waveInterval);

            // Spawn a Boss
            SpawnWaveEnemyBoss();
            // Increment wave number if there is a next wave
            if (waveIndex < level.waveList.Count-1)
            {
                waveIndex++;
                InitalizeCurrentWave(waveIndex);
                CalculateWaveQuota();

            }
            else
            {
                Debug.Log(" Last Wave Ended");
            }

        }

    }
    private IEnumerator BasicEnemySpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentWave.spawnInterval);

            int totalEnemyCount = EnemyManager.instance.activeEnemies.Count;


            // Trigger events when the number of active enemies exceeds the maxEnemyCount
            if (totalEnemyCount > level.maxActiveEnemyThreshold)
            {

                Debug.Log(" Trigger Special Events: Symptoms?");

                StopCoroutine(waveCoroutine);
                StopCoroutine(spawnCoroutine);

                //spawn boss or trigger
            }
            // Spawn enemy of each type if the number of enemies present in below the wave quota
            else if (currentWave.waveSpawnCounter < currentWave.waveSpawnQuota)
            {
                // Spawn each type of enemy
                foreach (EnemyGroup eg in currentWave.enemyGroups)
                {
                    // Spawn until quota is reached
                    if (eg.enemySpawnCounter < eg.enemyQuota)
                    {
                        SpawnEnemyBatch(1, eg.poolName);
                        eg.enemySpawnCounter++;
                        currentWave.waveSpawnCounter++;

                    }
                }
            }
            // Spawn random enemy if the enemies present are more than the wave quota
            else if (currentWave.waveSpawnCounter >= currentWave.waveSpawnQuota)
            {
                if (waveIndex < lastWaveIndex)
                {
                    int type = Random.Range(0, currentWave.enemyGroups.Count);

                    // spawn this type
                    SpawnEnemyBatch(1, currentWave.enemyGroups[type].poolName);
                    currentWave.enemyGroups[type].enemySpawnCounter++;
                    //currentWave.excessSpawnCounter++;                }
                }
                else
                {
                    StopCoroutine(waveCoroutine);
                    StopCoroutine(spawnCoroutine);
                }
            }
        }
    }
    private void SpawnWaveEnemyBoss()
    {
        foreach(BossEnemyGroup eb in currentWave.BossEnemyGroups)
        {
            for (int i = 0; i < eb.count; i++)
            {
                SpawnEnemyBatch(1, eb.poolName);
                Debug.Log(" Boss has been spawned");

            }
        }
    }

    private void SpawnEnemyBatch(int amount, string poolName)
    {
        GameObject player = GameManager.instance.Player;
        for (int i = 0; i < amount; i++)
        {
            // spawn point around the player
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 spawnPoint = player.transform.position + dir * Random.Range(30f, 40f);

            bool positiveX = Random.value < 0.5f;
            bool positiveZ = Random.value < 0.5f;

            //Vector3 spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 32f) : Random.Range(-9f, -10f));


            // Limit spawn position
            if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
            {
                spawnPoint = spawnPoint.normalized * maxSpawnDistance;
            }

            if (!spawnArea.bounds.Contains(spawnPoint))
            {
                angle = Random.Range(0f, 360f);
                dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                spawnPoint = player.transform.position + dir * Random.Range(30f, 40f);

                // Limit spawn position
                if (spawnPoint.sqrMagnitude > Mathf.Pow(maxSpawnDistance, 2))
                {
                    spawnPoint = spawnPoint.normalized * maxSpawnDistance;
                }

                //Debug.Log($" Enemy Out of bounds, moving to: {spawnPoint}");

            }
            else
            {
                SpawnEnemy(spawnPoint, poolName);

            }

            //float x = Random.Range(-50f, 50f);
            //float z = Random.Range(-50f, 50f);
        }
    }

    private void Update()
    {
    }

    public void SpawnEnemy(Vector3 position, string poolName)
    {
        GameObject enemy = RequestFromPool(position, poolName);
        //StartCoroutine(RelocateEnemy(enemy));

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

    private IEnumerator RelocateEnemies()
    {
        WaitForSeconds wait = new(1.0f);
        //WaitForFixedUpdate wait = new();

        while (this)
        {
            yield return wait;

            activeEnemies.RemoveAll((recruit) => !recruit.activeInHierarchy);
            activeEnemies.TrimExcess();

            foreach (GameObject enemy in activeEnemies)
            {
                // Translate World to Viewport
                Vector3 enemyPos = cam.WorldToViewportPoint(enemy.transform.position);

                Vector3 ViewportToWorldPos = enemyPos;

                // Proceed to next iteration if is within bounds
                if ((enemyPos.x >= minBounds.x && enemyPos.x <= maxBounds.x) && (enemyPos.y >= minBounds.y && enemyPos.y <= maxBounds.y))
                {
                    //Debug.Log("In range: " + enemyPos);

                    continue;
                }

                if (enemyPos.x < minBounds.x)
                {
                    // Left side of the screen
                    ViewportToWorldPos.x = -spawnThreshold;
                    ViewportToWorldPos.y = Random.Range(-spawnThreshold, 1 + spawnThreshold);
                }
                else if (enemyPos.x > maxBounds.x)
                {
                    // Right side of the screen
                    ViewportToWorldPos.x = 1 + spawnThreshold;
                    ViewportToWorldPos.y = Random.Range(-spawnThreshold, 1 + spawnThreshold); ;
                }
                else if (enemyPos.y < minBounds.y)
                {
                    // Bottom side of the screen
                    ViewportToWorldPos.y = -spawnThreshold;
                    ViewportToWorldPos.x = Random.Range(-spawnThreshold, 1 + spawnThreshold);
                }
                else if (enemyPos.y > maxBounds.y)
                {
                    // Top side of the screen
                    ViewportToWorldPos.y = 1 + spawnThreshold;
                    ViewportToWorldPos.x = Random.Range(-spawnThreshold, 1 + spawnThreshold);
                }

                Vector3 newPos = cam.ViewportToWorldPoint(new Vector3(ViewportToWorldPos.x, ViewportToWorldPos.y, ViewportToWorldPos.z));

                enemy.transform.position = new Vector3(newPos.x, enemy.transform.position.y, newPos.z);

                //Debug.Log("Not in range: " + enemyPos);

            }

            //foreach (var recruit in activeRecruits)
            //{
            //    if (Vector3.Distance(player.transform.position, recruit.transform.position) > 30f)
            //    {
            //        Vector3 spawnPoint = (recruit.transform.position - player.transform.position).normalized;

            //        if (spawnPoint.z > 0f)
            //        {
            //            spawnPoint *= 30f;
            //        }
            //        else
            //        {
            //            spawnPoint *= 12f;
            //        }

            //        //bool positiveX = Random.value < 0.5f;
            //        //bool positiveZ = Random.value < 0.5f;

            //        //Vector3 spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 31f) : Random.Range(-9f, -10f));

            //        //while (!spawnArea.bounds.Contains(spawnPoint))
            //        //{
            //        //    positiveX = Random.value < 0.5f;
            //        //    positiveZ = Random.value < 0.5f;

            //        //    spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 31f) : Random.Range(-9f, -10f));
            //        //    Debug.Log($"Out of bounds, moving to: {spawnPoint}"); 
            //        //    //yield return null;
            //        //}

            //        recruit.transform.position = player.transform.position + spawnPoint;
            //        Debug.Log("Relocated " + recruit.name);
            //    }
            //}

        }
    }

    public IEnumerator RelocateEnemy(GameObject enemy)
    {
        Vector3 enemyPos = cam.WorldToViewportPoint(enemy.transform.position);

        yield return new WaitForSeconds(1.0f);
        // Translate World to Viewport
        enemyPos = cam.WorldToViewportPoint(enemy.transform.position);

        Vector3 ViewportToWorldPos = enemyPos;

        // Proceed to next iteration if is within bounds
        //if ((enemyPos.x >= minBounds.x && enemyPos.x <= maxBounds.x) && (enemyPos.y >= minBounds.y && enemyPos.y <= maxBounds.y))
        //{
        //    Debug.Log("In range: " + enemyPos);

            //    return;
            //}
        if (enemyPos.y > maxBounds.y)
        {
            // Top side of the screen
            ViewportToWorldPos.y = 0.6f;
            ViewportToWorldPos.x = Random.Range(0f, 1f);
        }
        else if (enemyPos.y < minBounds.y)
        {
            // Bottom side of the screen
            ViewportToWorldPos.y = 0.6f;
            ViewportToWorldPos.x = Random.Range(0f, 1f);
        }

        //enemyPos = cam.WorldToViewportPoint(cam.ViewportToWorldPoint(new Vector3(ViewportToWorldPos.x, ViewportToWorldPos.y, ViewportToWorldPos.z)));

        else if (enemyPos.x < minBounds.x)
        {
            // Left side of the screen
            ViewportToWorldPos.x = 0f;
            ViewportToWorldPos.y = Random.Range(0f, 1f);
        }
        else if (enemyPos.x > maxBounds.x)
        {
            // Right side of the screen
            ViewportToWorldPos.x = 1f;
            ViewportToWorldPos.y = Random.Range(0f, 1f);
        }

        Vector3 newPos = cam.ViewportToWorldPoint(new Vector3(ViewportToWorldPos.x, ViewportToWorldPos.y, ViewportToWorldPos.z));

        enemy.transform.position = new Vector3(newPos.x, enemy.transform.position.y, newPos.z);

        //Debug.Log("Not in range: " + enemyPos + " New pos: " + cam.WorldToViewportPoint(enemy.transform.position));
    }

    public GameObject RequestFromPool(Vector3 position, string poolName)
    {
        foreach(EnemyPool enpool in enemyPools )
        {
            if (enpool.Name == poolName)
            {
                return enpool.enemyPool.RequestPoolable(position);
            }
        }
        return null;
    }
    private void InitalizeCurrentWave(int waveNum)
    {
        level.waveList[waveNum].waveSpawnCounter = 0;
        foreach (EnemyGroup eg in level.waveList[waveNum].enemyGroups)
        {
            eg.enemySpawnCounter = 0;
        }
        currentWave = level.waveList[waveNum];
    }
    private void CalculateWaveQuota()
    {
        
        int currentWaveQuota = 0;

        // Current wave quota  = accumulative sum of the enemy groups in the current wave
        foreach (EnemyGroup eg in currentWave.enemyGroups)
        {
            currentWaveQuota += eg.enemyQuota;
        }

        currentWave.waveSpawnQuota = currentWaveQuota;
        //Debug.Log("Wave " + currentWave.waveName + " Quota:" + currentWaveQuota);
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
