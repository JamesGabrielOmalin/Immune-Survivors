using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeSpan;
    public float projectileSpeed;

    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        StartCoroutine(LifeSpanCoroutine());
    }

    protected virtual void FixedUpdate()
    {
        transform.position += transform.forward * (projectileSpeed * Time.fixedDeltaTime);
    }

    protected IEnumerator LifeSpanCoroutine()
    {
        yield return new WaitForSeconds(lifeSpan);
        this.gameObject.SetActive(false);
    }
}
