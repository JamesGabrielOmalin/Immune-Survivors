using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DamageNumber : MonoBehaviour
{
    public TextMeshPro text;
    [SerializeField] private float despawnTime;

    private void OnEnable()
    {
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);

        this.gameObject.SetActive(false);
    }
}
