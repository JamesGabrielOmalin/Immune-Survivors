using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMarker : MonoBehaviour
{
    public GameObject target;
    public bool isActive = false;
    // Start is called before the first frame update


    public void TrackUnit(GameObject unit)
    {
        target = unit;

    }

    public void UntrackUnit(GameObject unit)
    {
        target = unit;

    }
}
