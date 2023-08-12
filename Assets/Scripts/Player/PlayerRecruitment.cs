using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecruitment : MonoBehaviour, IBodyColliderListener
{
    [SerializeField] private Player owningPlayer;

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerUnit>(out PlayerUnit recruit))
        {
            owningPlayer.RecruitUnit(recruit);
            recruit.gameObject.SetActive(false);
            WaypointMarkerManager.instance.UnregisterToWaypointMarker(recruit.gameObject);

        }
    }

    public void OnBodyColliderExit(Collider other)
    {
        //
    }
}
