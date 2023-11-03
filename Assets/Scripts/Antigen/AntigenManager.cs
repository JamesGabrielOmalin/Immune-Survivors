using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntigenManager : MonoBehaviour
{
    public static AntigenManager instance;

    [SerializeField] private List<ObjectPool> antigenPools = new();

    [SerializeField] private int antigenThreshold;

    public System.Action OnAntigenPickup;

    private readonly Dictionary<AntigenType, int> antigenCount = new()
    {
        { AntigenType.Type_1, 0 },
        { AntigenType.Type_2, 0 },
        { AntigenType.Type_3, 0 },
    };

    private readonly Dictionary<AntigenType, int> antigenCountTotal = new()
    {
        { AntigenType.Type_1, 0 },
        { AntigenType.Type_2, 0 },
        { AntigenType.Type_3, 0 },
    };

    public readonly Dictionary<AntigenType, System.Action> OnAntigenCountChanged = new()
    {
        { AntigenType.Type_1, null},
        { AntigenType.Type_2, null },
        { AntigenType.Type_3, null },
    };

    public readonly Dictionary<AntigenType, System.Action> OnAntigenThresholdReached = new()
    {
        { AntigenType.Type_1, null },
        { AntigenType.Type_2, null },
        { AntigenType.Type_3, null },
    };

    private bool firstTimeAntigenThresholdReached = true;

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

    public GameObject SpawnAntigen(Vector3 position, AntigenType type)
    {
        return antigenPools[(int)type].RequestPoolable(position);
    }

    public void AddAntigen(AntigenType type)
    {
        antigenCount[type] += 10;
        antigenCountTotal[type] += 10;
        OnAntigenCountChanged[type]?.Invoke();

        if (antigenCount[type] >= antigenThreshold && OnAntigenThresholdReached[type] != null)
        {
            OnAntigenThresholdReached[type]?.Invoke();
            //OnAntigenThresholdReached[type] = null;
            antigenCount[type] = 0;
        }
    }

    public void AddBonusAntigen(AntigenType type)
    {
        antigenCount[type] += 5;
        antigenCountTotal[type] += 5;
        OnAntigenCountChanged[type]?.Invoke();

        if (antigenCount[type] >= antigenThreshold && OnAntigenThresholdReached[type] != null)
        {
            OnAntigenThresholdReached[type]?.Invoke();
            //OnAntigenThresholdReached[type] = null;
            antigenCount[type] = 0;
        }
    }

    public int GetAntigenCount(AntigenType type)
    {
        return antigenCountTotal[type];
    }
}
