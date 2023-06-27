using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeSpan;
    public float projectileSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LifeSpanCoroutine());
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * (projectileSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator LifeSpanCoroutine()
    {
        yield return new WaitForSeconds(lifeSpan);
        this.gameObject.SetActive(false);
    }
}
