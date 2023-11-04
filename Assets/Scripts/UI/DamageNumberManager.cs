using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager instance;

    [SerializeField] private ObjectPool damageNumberPool;

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

    public void SpawnDamageNumber(in Vector3 position, in float amount, bool isCrit)
    {
        GameObject damageNumber = damageNumberPool.RequestPoolable(position);

        if (!damageNumber)
        {
            Debug.LogWarning("No damageNumber found in object pool!");
            return;
        }

        DamageNumber text = damageNumber.GetComponent<DamageNumber>();

        if (isCrit)
        {
            text.text.fontSize = 1f;
            text.text.text = $"<color=red>{Mathf.CeilToInt(amount)}!</color>";
            text.text.sortingOrder = 5;
        }
        else
        {
            text.text.fontSize = 0.75f;
            text.text.text = $"{Mathf.CeilToInt(amount)}";
            text.text.sortingOrder = 4;
        }
    }

    public void SpawnDoTNumber(in Vector3 position, in float amount)
    {
        GameObject damageNumber = damageNumberPool.RequestPoolable(position);

        if (!damageNumber)
        {
            Debug.LogWarning("No damageNumber found in object pool!");
            return;
        }

        DamageNumber text = damageNumber.GetComponent<DamageNumber>();
        text.text.fontSize = 0.5f;
        text.text.text = $"{Mathf.CeilToInt(amount)}";
        text.text.sortingOrder = 3;
    }

    public void SpawnHealNumber(in Vector3 position, in float amount)
    {
        GameObject damageNumber = damageNumberPool.RequestPoolable(position);

        if (!damageNumber)
        {
            Debug.LogWarning("No damageNumber found in object pool!");
            return;
        }

        DamageNumber text = damageNumber.GetComponent<DamageNumber>();
        text.text.fontSize = 1f;
        text.text.text = $"<color=#8DFF22>+{Mathf.CeilToInt(amount)}</color>";
        text.text.sortingOrder = 3;
    }
}
