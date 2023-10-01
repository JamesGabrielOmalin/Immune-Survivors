using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperTCell : AdaptiveCell
{
    private const float SPAWN_TIME = 5f;

    [SerializeField] private Image spawnTimerIndicator;

    private void Start()
    {
        StartCoroutine(Spawn());   
    }

    private IEnumerator Spawn()
    {
        ObjectPool cytokinePool = AdaptiveManager.instance.cytokinePool;
        WaitForSeconds wait = new WaitForSeconds(SPAWN_TIME);
        while (this)
        {
            //for (int i = 0; i < 12; i++)
            //{
            //    float angle = (i * 30f) * Mathf.Deg2Rad;
            //    Vector3 dir = new(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

            //    GameObject cytokine = cytokinePool.RequestPoolable(transform.position);

            //    if (!cytokine)
            //        continue;
            //    cytokine.transform.forward = dir;

            //    cytokine.GetComponent<Cytokine>().SetType(Type);
            //}

            float t = 0f;

            while (t < SPAWN_TIME)
            {
                t += Time.deltaTime;
                spawnTimerIndicator.fillAmount = t / SPAWN_TIME;
                yield return null;
            }

            GameObject cytokine = cytokinePool.RequestPoolable(transform.position + Vector3.up);
            cytokine.GetComponent<Cytokine>().SetType(Type);

            yield return new WaitUntil(() => !cytokine.activeInHierarchy);
        }
    }
}
