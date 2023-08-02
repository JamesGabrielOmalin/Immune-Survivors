using System.Collections;
using System.Collections.Generic;
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
    [field: SerializeField]
    public int InitialAmountToSpawn { get; private set; }

    [field: SerializeField]
    public int AmountPerBatch { get; private set; }
    [field: SerializeField]
    public float BatchSpawnInterval { get; private set; }
    [SerializeField] private AnimationCurve spawnIntervalCurve = AnimationCurve.Linear(0f, 0f, 10f, 1f);
    [SerializeField] private AnimationCurve spawnAmountCurve = AnimationCurve.Linear(0f, 0f, 10f, 1f);

    [SerializeField] private float maxSpawnDistance;

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
        SpawnEnemyBatch(InitialAmountToSpawn);
        StartCoroutine(SpawnCoroutine());
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(BatchSpawnInterval);
            float minutes = (float)GameManager.instance.GameTime.TotalMinutes;
            BatchSpawnInterval = spawnIntervalCurve.Evaluate(minutes);
            AmountPerBatch = (int)spawnAmountCurve.Evaluate(minutes);
            SpawnEnemyBatch(AmountPerBatch);
        }
    }

    private void SpawnEnemyBatch(int amount)
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

            SpawnEnemy(spawnPoint, (AntigenType)Random.Range(0, 3));
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

    // Return nearest enemy from the active enemy list
    public GameObject GetNearestEnemy(in Vector3 position, float limit = float.MaxValue)
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
