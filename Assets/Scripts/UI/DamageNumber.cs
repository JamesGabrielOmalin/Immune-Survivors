using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DamageNumber : MonoBehaviour
{
    public TextMeshPro text;
    [SerializeField] private float speed;
    [SerializeField] private float despawnTime;

    private void OnEnable()
    {
        StartCoroutine(Despawn());
    }

    //private void FixedUpdate()
    //{
    //    transform.position += Vector3.up *(speed * Time.fixedDeltaTime);
    //}

    //public void StartDespawnTimer()
    //{
    //    StartCoroutine(Despawn());
    //}

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);

        this.gameObject.SetActive(false);
    }
}
