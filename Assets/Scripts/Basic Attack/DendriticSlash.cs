using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DendriticSlash : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    //[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;
    [HideInInspector] public int slashCount;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Slash());
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(1f / slashCount);
        vfx.SetInt("Count", slashCount);
        vfx.Play();

        for (int i = 0; i < slashCount; i++)
        {
            //sprite.gameObject.SetActive(false);
            float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

            var hits = Physics.OverlapSphere(transform.position, transform.localScale.x, layerMask.value);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }

            yield return wait;
            //sprite.gameObject.SetActive(true);
        }

        yield return null;
        Destroy(this.gameObject);
        yield break;
    }
}
