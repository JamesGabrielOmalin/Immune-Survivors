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
        text.text.text = Mathf.CeilToInt(amount).ToString();
        
        if (isCrit)
        {
            text.text.color = Color.red;
            text.text.fontSize = 0.6f;
        }
        else
        {
            text.text.color = Color.white;
            text.text.fontSize = 0.5f;
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
        text.text.text = Mathf.CeilToInt(amount).ToString();

        text.text.color = Color.white;
        text.text.fontSize = 0.3f;
    }
}
