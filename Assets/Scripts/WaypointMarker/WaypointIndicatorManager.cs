using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class WaypointIndicatorManager : MonoBehaviour
{
    public static WaypointIndicatorManager instance;

    [SerializeField] private ObjectPool indicatorPool;
    public List<WaypointIndicator> activeIndicators = new();


    [Header("Offset")]
    [SerializeField] float offset;

    private Coroutine trackingCoroutine;

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
    void Start()
    {
        trackingCoroutine = StartCoroutine(IndicatorCoroutine());
    }
    private void OnDisable()
    {
        StopCoroutine(trackingCoroutine);

    }

    private void OnDestroy()
    {
        instance = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RegisterToWaypointMarker(GameObject unit)
    {
        GameObject indicator = indicatorPool.RequestPoolable(indicatorPool.transform.position);
        indicator.transform.SetParent(indicatorPool.transform);
        indicator.transform.localPosition = Vector3.zero;

        if (!indicator)
        {
            Debug.LogWarning("No indicator found in object pool!");
            return;
        }


        if (indicator.TryGetComponent<WaypointIndicator>(out WaypointIndicator indicatorComp))
        {
            activeIndicators.Add(indicatorComp);
            indicatorComp.TrackUnit(unit);
            indicatorComp.screenOffset = offset;
            Debug.Log(" Unit registered");

            //indicator.SetActive(true);



        }
        else
        {
            Debug.LogWarning(" No indicator component found!");
        }

    }

    public void UnregisterToWaypointMarker(GameObject unit)
    {
        foreach (WaypointIndicator indicator in activeIndicators)
        {
            if (indicator.target == unit.transform)
            {
                indicator.UntrackUnit();
                indicator.gameObject.SetActive(false);
                Debug.Log("Indicator has been unregistered");
            }
        }

        activeIndicators.RemoveAll((indicator) => indicator.target == null);
        activeIndicators.TrimExcess();
    }

    IEnumerator IndicatorCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            foreach (WaypointIndicator indicator in activeIndicators)
            {
                if (indicator.target !=  null) 
                {
                    if (!indicator.gameObject.activeInHierarchy)
                    {
                        indicator.gameObject.SetActive(true);
                    }
                    indicator.UpdateIndicator();
                }
            }
        }

    }
}
