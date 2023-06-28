using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperTCell : AdaptiveCell
{
    private void Start()
    {
        StartCoroutine(Spawn());   
    }

    private IEnumerator Spawn()
    {
        ObjectPool cytokinePool = AdaptiveManager.instance.cytokinePool;
        WaitForSeconds wait = new WaitForSeconds(5f);
        while (this)
        {
            for (int i = 0; i < 12; i++)
            {
                float angle = (i * 30f) * Mathf.Deg2Rad;
                Vector3 dir = new(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

                GameObject cytokine = cytokinePool.RequestPoolable(transform.position);
                
                if (!cytokine)
                    continue;
                cytokine.transform.forward = dir;

                cytokine.GetComponent<Cytokine>().SetType(Type);
            }

            yield return wait;
        }
    }
}
