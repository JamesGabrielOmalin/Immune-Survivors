using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class KillThreshold
{
    [field: SerializeField]
    public int startCycle;
    [field: SerializeField]
    public int endCycle;
    [field: SerializeField]

    public int increment;
    [field: SerializeField]

    public int nRecruitToSpawn;
}

public class RecruitManager : MonoBehaviour
{

    public static RecruitManager instance;

    public System.Action OnThreshholdUpdate;

    [SerializeField] private List<ObjectPool> recruitPools = new();
    private readonly List<GameObject> activeRecruits = new();
    GameObject player;

    [Header("Base Spawning Attributes")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private int initialAmountToSpawn;

    [Header(" Timed Spawning")]
    [SerializeField] private bool EnableIntervalSpawning = false;

    [SerializeField] private float spawnInterval;
    [SerializeField] private int amountToSpawnPerInterval;

    [Header(" Threshold Spawning")]
    [SerializeField] private bool EnableThresholdSpawning = false;
    private int index = 0;
    [SerializeField] private int initialKillRequirement;

    [Header(" Threshold Stats")]

    public int killCount = 0;
    [SerializeField] private int killThreshold;
    [SerializeField] private int cycle = 1;
    [SerializeField] private int thresholdIncrement;
    [SerializeField] private int amountToSpawn;

    [field: SerializeField]
    public List<KillThreshold> killThresholdList;

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
        player = GameManager.instance.Player;

        killThreshold = initialKillRequirement;
        thresholdIncrement = killThresholdList[index].increment;

        SpawnRecruitBatch(initialAmountToSpawn);

        if (EnableIntervalSpawning)
        {
            StartCoroutine(SpawnCoroutine());
        }

        StartCoroutine(RelocateRecruits());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnRecruitBatch(amountToSpawnPerInterval);
        }
    }

    private void SpawnRecruitBatch(int amount)
    {
        //float angle;
        //Vector3 dir;
        for (int i = 0; i < amount; i++)
        {
            // spawn point around the player
            //angle = Random.Range(0f, 360f);

            //dir = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            //Vector3 spawnPoint = RandomPointInBounds(spawnArea.bounds);

            //spawnPoint = new Vector3(spawnPoint.x, 0, spawnPoint.z);

            bool positiveX = Random.value < 0.5f;
            bool positiveZ = Random.value < 0.5f;

            Vector3 spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 32f) : Random.Range(-9f, -10f));

            while (!spawnArea.bounds.Contains(spawnPoint))
            {
                positiveX = Random.value < 0.5f;
                positiveZ = Random.value < 0.5f;

                spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 32f) : Random.Range(-9f, -10f));
                
                Debug.Log($"Out of bounds, moving to: {spawnPoint}");
            }

            PlayerUnitType toSpawn = (PlayerUnitType)Random.Range(0, 3);
            //float rand = Random.value;

            //if (rand < 0.1f)
            //{
            //    toSpawn = PlayerUnitType.Dendritic;
            //}
            //else if (rand < 0.3f)
            //{
            //    toSpawn = PlayerUnitType.Macrophage;
            //}
            //else if (rand < 0.7f)
            //{
            //    toSpawn = PlayerUnitType.Neutrophil;
            //}

            SpawnRecruit(spawnPoint, toSpawn);
        }
    }

    private Vector3 RandomPointInBounds(in Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void SpawnRecruit(Vector3 position, PlayerUnitType type)
    {
        GameObject recruit = recruitPools[(int)type].RequestPoolable(position);

        if (!recruit)
        {
            Debug.LogWarning("No recruit found in object pool!");
            return;
        }

        Debug.Log("Recruit Spawned");
        activeRecruits.Add(recruit);
        WaypointMarkerManager.instance.RegisterToWaypointMarker(recruit);
    }

    public void AddKillCount()
    {
        if (!EnableThresholdSpawning)
        {
            return;
        }


        //cycle = (int)GameManager.instance.Player.GetComponent<Player>().GetActiveUnit().GetComponent<AttributeSet>().GetAttribute("Level").Value;

        // next set of values
        if (cycle > killThresholdList[index].endCycle)
        {
            if (index + 1 <= killThresholdList.Count)
            {
                Debug.Log("Next Kill Requirement");
                if (index+1 < killThresholdList.Count)
                {
                    index++;

                }
            }
        }
        
        if (cycle >= killThresholdList[index].startCycle && cycle <= killThresholdList[index].endCycle )
        {
            killCount++;

            amountToSpawn = killThresholdList[index].nRecruitToSpawn;

            // spawn units when kill requirement is reached
            if (killCount >= killThreshold)
            {
                SpawnRecruitBatch(killThresholdList[index].nRecruitToSpawn);
                thresholdIncrement = killThresholdList[index].increment;
                killThreshold += killThresholdList[index].increment;
                killCount = 0;
                cycle++;
            }
        }

        OnThreshholdUpdate?.Invoke();
    }

    private IEnumerator RelocateRecruits()
    {
        WaitForSeconds wait = new(30f);
        while (this)
        {
            yield return wait;

            activeRecruits.RemoveAll((recruit) => !recruit.activeInHierarchy);
            activeRecruits.TrimExcess();

            foreach (var recruit in activeRecruits)
            {
                if (Vector3.Distance(player.transform.position, recruit.transform.position) > 30f)
                {
                    Vector3 spawnPoint = (recruit.transform.position - player.transform.position).normalized;

                    if (spawnPoint.z > 0f)
                    {
                        spawnPoint *= 30f;
                    }
                    else
                    {
                        spawnPoint *= 12f;
                    }

                    //bool positiveX = Random.value < 0.5f;
                    //bool positiveZ = Random.value < 0.5f;

                    //Vector3 spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 31f) : Random.Range(-9f, -10f));
                    
                    //while (!spawnArea.bounds.Contains(spawnPoint))
                    //{
                    //    positiveX = Random.value < 0.5f;
                    //    positiveZ = Random.value < 0.5f;

                    //    spawnPoint = player.transform.position + new Vector3(Random.Range(16f, 17f) * (positiveX ? 1 : -1), 0f, positiveZ ? Random.Range(30f, 31f) : Random.Range(-9f, -10f));
                    //    Debug.Log($"Out of bounds, moving to: {spawnPoint}"); 
                    //    //yield return null;
                    //}
                    
                    recruit.transform.position = player.transform.position + spawnPoint;
                    Debug.Log("Relocated " + recruit.name);
                }
            }
        }
    }

    public int GetCurrentKillThreshold()
    {
        return killThreshold;
    }
}
