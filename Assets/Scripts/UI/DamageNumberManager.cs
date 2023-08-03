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

    public void SpawnDamageNumber(Vector3 position, float amount)
    {
        GameObject damageNumber = damageNumberPool.RequestPoolable(position);

        if (!damageNumber)
        {
            Debug.LogWarning("No damageNumber found in object pool!");
            return;
        }

        damageNumber.GetComponent<TMP_Text>().text = amount.ToString();
        damageNumber.GetComponent<DamageNumber>().startDespawnTimer();
    }
}
