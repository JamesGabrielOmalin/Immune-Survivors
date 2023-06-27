using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCell : AdaptiveCell
{
    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        ObjectPool antibodyPool = AdaptiveManager.instance.antibodyPool;
        WaitForSeconds wait = new(5f);
        while (this)
        {
            for (int i = 0; i < 12; i++)
            {
                float angle = (i * 30f) * Mathf.Deg2Rad;
                Vector3 dir = new(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

                GameObject antibody = antibodyPool.RequestPoolable(transform.position);
                antibody.transform.forward = dir;

                antibody.GetComponent<Antibody>().SetType(Type);
            }

            yield return wait;
        }
    }
}
