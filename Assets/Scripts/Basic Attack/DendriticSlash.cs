using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DendriticSlash : MonoBehaviour
{
    [HideInInspector] public Enemy target;
    //[SerializeField] private LayerMask layerMask;
    //[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private VisualEffect vfx;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public int attackCount;
    [HideInInspector] public float critRate;
    [HideInInspector] public float critDMG;

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(Slash());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Slash()
    {
        WaitForSeconds wait = new(1f / attackCount);
        vfx.SetInt("Count", attackCount);
        vfx.Play();

        for (int i = 0; i < attackCount; i++)
        {
            //sprite.gameObject.SetActive(false);
            float damage = DamageCalculator.CalcDamage(attackDamage, critRate, critDMG);

            target.TakeDamage(damage);

            yield return wait;
            //sprite.gameObject.SetActive(true);
        }

        yield return null;
        this.gameObject.SetActive(false);
        yield break;
    }
}
