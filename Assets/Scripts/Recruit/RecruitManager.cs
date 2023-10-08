
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

    public System.Action OnKillCountUpdate;
    public System.Action OnThreshholdUpdate;
    public System.Action OnRecruitSpawn;


    private Camera cam;

    [SerializeField] private List<ObjectPool> recruitPools = new();
    private readonly List<GameObject> activeRecruits = new();
    GameObject player;

    [Header("Base Spawning Attributes")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private int initialAmountToSpawn;
    [SerializeField] private LayerMask layersToCheck;
    [SerializeField] private float collisionCheckerRadius;
    [SerializeField] private int maxChecks;



    [Header(" Timed Spawning")]
    [SerializeField] private bool EnableIntervalSpawning = false;

    [SerializeField] private float spawnInterval;
    [SerializeField] private int amountToSpawnPerInterval;

    [Header(" Threshold Spawning")]
    [SerializeField] private bool EnableThresholdSpawning = false;
    private int index = 0;
    [SerializeField] private int initialKillRequirement;

    [Header(" Threshold Stats")]

    public int totalKillCount = 0;
    public int killCount = 0;
    [SerializeField] private int killThreshold;
    [SerializeField] private int cycle = 1;
    [SerializeField] private int thresholdIncrement;
    [SerializeField] private int amountToSpawn;

    [field: SerializeField]
    public List<KillThreshold> killThresholdList;


    [Header("Viewport Bounds Threshold")]
    
    [Tooltip("Relocate objects when the x units away from the viewport screen")]
    [SerializeField] private Vector2 boundsThreshold;

    [Tooltip("Relocate objects by x units outside of the viewport screen (must be less than bouds threshold)")]
    [SerializeField] private float spawnThreshold;

    [Tooltip("Within (0-1)  LL Viewing fustrum (0,0) ")]
    private Vector2 minBounds = new Vector2(0,0);
    [Tooltip("Within (0-1)  UR Viewing fustrum (1,1) ")]
    private Vector2 maxBounds = new Vector2(1,1);

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
        cam = Camera.main;
        player = GameManager.instance.Player;

        minBounds-= boundsThreshold;
        maxBounds += boundsThreshold;
        

        killThreshold = initialKillRequirement;
        thresholdIncrement = killThresholdList[index].increment;

        SpawnRecruitBatch(initialAmountToSpawn);

        if (EnableIntervalSpawning)
        {
            StartCoroutine(SpawnCoroutine());
        }

        //StartCoroutine(RelocateRecruits());
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

        for (int i = 0; i < amount; i++)
        {

            Vector3 spawnPoint = RandomPointInBounds(spawnArea.bounds);
            spawnPoint.y = 0;
 

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

            StartCoroutine(SpawnRecruit(spawnPoint, toSpawn));
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

    public IEnumerator SpawnRecruit(Vector3 position, PlayerUnitType type)
    {

        yield return new WaitForSeconds(0.25f);

        bool canSpawn = false;
        int nChecks = 0;
        do
        {
            bool isColliding = Physics.CheckSphere(position, collisionCheckerRadius, layersToCheck);

            if (isColliding)
            {
                //Debug.Log("has Collision");
                position = RandomPointInBounds(spawnArea.bounds);
                position.y = 0;
                nChecks++;
            }
            else
            {
                canSpawn = true;
            }
            //Debug.Log(isColliding);

        } while (!canSpawn && nChecks < maxChecks);

        GameObject recruit = recruitPools[(int)type].RequestPoolable(position);

        if (!recruit)
        {
            Debug.LogWarning("No recruit found in object pool!");
            yield return null;
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
            totalKillCount++;
            OnKillCountUpdate?.Invoke();
            amountToSpawn = killThresholdList[index].nRecruitToSpawn;

            // spawn units when kill requirement is reached
            if (killCount >= killThreshold)
            {
                SpawnRecruitBatch(killThresholdList[index].nRecruitToSpawn);
                thresholdIncrement = killThresholdList[index].increment;
                killThreshold += killThresholdList[index].increment;
                killCount = 0;
                cycle++;
                OnRecruitSpawn?.Invoke();
            }
        }

        OnThreshholdUpdate?.Invoke();
    }

    private IEnumerator RelocateRecruits()
    {
        WaitForSeconds wait = new(1.0f);
        //WaitForFixedUpdate wait = new();

        while (this)
        {
            yield return wait;

            activeRecruits.RemoveAll((recruit) => !recruit.activeInHierarchy);
            activeRecruits.TrimExcess();

            foreach (GameObject recruit in activeRecruits)
            {
                // Translate World to Viewport
                Vector3 recruitPos = cam.WorldToViewportPoint(recruit.transform.position);

                Vector3 ViewportToWorldPos = recruitPos;

                // Proceed to next iteration if is within bounds
                if ((recruitPos.x >= minBounds.x && recruitPos.x <= maxBounds.x) && (recruitPos.y >= minBounds.y && recruitPos.y <= maxBounds.y))
                {
                    //Debug.Log("In range: " + recruitPos);

                    continue;
                }

                if (recruitPos.x < minBounds.x)
                {
                    // Left side of the screen
                    ViewportToWorldPos.x = -spawnThreshold;
                    ViewportToWorldPos.y = Random.Range(-spawnThreshold,1 + spawnThreshold);
                }
                else if (recruitPos.x > maxBounds.x)
                {
                    // Right side of the screen
                    ViewportToWorldPos.x = 1+spawnThreshold;
                    ViewportToWorldPos.y = Random.Range(-spawnThreshold, 1 + spawnThreshold); ;
                }
                else if (recruitPos.y < minBounds.y)
                {
                    // Bottom side of the screen
                    ViewportToWorldPos.y = -spawnThreshold;
                    ViewportToWorldPos.x = Random.Range(-spawnThreshold, 1 + spawnThreshold);
                }
                else if (recruitPos.y > maxBounds.y)
                {
                    // Top side of the screen
                    ViewportToWorldPos.y = 1 + spawnThreshold;
                    ViewportToWorldPos.x = Random.Range(-spawnThreshold, 1 + spawnThreshold);
                }

                Vector3 newPos = cam.ViewportToWorldPoint(new Vector3(ViewportToWorldPos.x, ViewportToWorldPos.y, ViewportToWorldPos.z));

                recruit.transform.position = new Vector3(newPos.x, recruit.transform.position.y, newPos.z);

                //Debug.Log("Not in range: " + recruitPos);

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

    public int GetCurrentKillThreshold()
    {
        return killThreshold;
    }
    
}
