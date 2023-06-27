using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DendriticSlash : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private SpriteRenderer sprite;
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public int slashCount;
    [HideInInspector] public float slashSize;

    // Start is called before the first frame update
    private void Start()
    {
        transform.localScale = Vector3.one * slashSize;
        StartCoroutine(Slash());
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(0.25f / slashCount);

        for (int i = 0; i < slashCount; i++)
        {
            sprite.gameObject.SetActive(false);
            float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

            var hits = Physics.OverlapSphere(transform.position, slashSize, layerMask.value);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }

            yield return wait;
            sprite.gameObject.SetActive(true);
            yield return null;
        }

        Destroy(this.gameObject);
        yield break;
    }
}
