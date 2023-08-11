using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class WaypointMarkerManager : MonoBehaviour
{
    public static WaypointMarkerManager instance;

    [SerializeField] private ObjectPool markerPool;
    public List<WaypointMarker> activeMarkers = new();


    [Header("Bounds")]
    [Tooltip("Within (0-1)")]
    [SerializeField] private Vector2 minBounds;
    [Tooltip("Within (0-1)")]
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;

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
        cam = Camera.main;
        StartCoroutine(WaypointCoroutine());
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
        GameObject marker = markerPool.RequestPoolable(markerPool.transform.position);
        marker.transform.SetParent(markerPool.transform);
        marker.transform.localPosition = Vector3.zero;

        if (!marker)
        {
            Debug.LogWarning("No marker found in object pool!");
            return;
        }

        marker.SetActive(true);

        if (marker.TryGetComponent<WaypointMarker>(out WaypointMarker markerComp))
        {
            activeMarkers.Add(markerComp);
            markerComp.TrackUnit(unit);
            markerComp.isActive= true;
            Debug.Log(" Unit registered");


        }
        else
        {
            Debug.LogWarning(" No marker component found!");
        }

    }

    public void UnregisterToWaypointMarker(GameObject unit)
    {
        foreach(WaypointMarker marker in activeMarkers)
        {
            if (marker.target == unit)
            {
                marker.target = null;
                marker.isActive = false;
                marker.gameObject.SetActive(false);
                //activeMarkers.Remove(marker);
                Debug.Log("Marker has been unregistered");
            }
        }

        activeMarkers.RemoveAll((marker) => marker.target == null);
        activeMarkers.TrimExcess();
    }


    private IEnumerator WaypointCoroutine()
    {
        WaitForFixedUpdate wait = new();
        while (true)
        {
            yield return wait;

            foreach(WaypointMarker marker in activeMarkers)
            {

                Vector3 targetPos = cam.WorldToViewportPoint(marker.target.transform.position);


                if (targetPos.x < minBounds.x || targetPos.x > maxBounds.x ||
                     targetPos.y < minBounds.y || targetPos.y > maxBounds.y)
                {
                    Debug.Log("Not in range");
                    marker.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("In range");
                    marker.gameObject.SetActive(false);

                }

                Vector3 targetDir = marker.target.transform.position - marker.transform.position;

                Vector3 newDirection = Vector3.RotateTowards(marker.gameObject.transform.forward, targetDir, Time.fixedDeltaTime, 0.0f);

                marker.gameObject.transform.rotation = Quaternion.LookRotation(newDirection);
            }

        }
    }
}
