using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float despawnTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void startDespawnTimer()
    {
        StartCoroutine(despawn());
    }

    IEnumerator despawn()
    {
        yield return new WaitForSeconds(despawnTime);

        this.gameObject.SetActive(false);
    }
}
