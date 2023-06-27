using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecruitment : MonoBehaviour, IBodyColliderListener
{
    [SerializeField] private Player owningPlayer;

    // Start is called before the first frame update
    private void Start()
    {

    }

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerUnit>(out PlayerUnit recruit))
        {
            owningPlayer.RecruitUnit(recruit);
            recruit.gameObject.SetActive(false);
        }
    }

    public void OnBodyColliderExit(Collider other)
    {
        //
    }
}
