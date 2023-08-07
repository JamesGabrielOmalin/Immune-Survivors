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

        TMP_Text text = damageNumber.GetComponent<TMP_Text>();
        text.text = Mathf.RoundToInt(amount).ToString();
        text.color = Color.white;
        text.fontSize = 0.5f;

        if (isCrit)
        {
            text.color = Color.red;
            text.fontSize = 0.6f;
        }
    }
}
