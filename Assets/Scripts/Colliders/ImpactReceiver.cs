using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [HideInInspector] public bool hasImpact;
    private readonly WaitForSeconds impactDuration = new(0.25f);

    private Coroutine impactCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        if (!rigidBody)
            rigidBody = GetComponent<Rigidbody>();
    }

    public void AddImpact(Vector3 dir, float force)
    {
        rigidBody.AddForce(dir * force, ForceMode.VelocityChange);
        hasImpact = true;

        if (impactCoroutine != null)
            StopCoroutine(impactCoroutine);
        impactCoroutine = StartCoroutine(Impact());
    }

    private IEnumerator Impact()
    {
        hasImpact = true;
        yield return impactDuration;
        hasImpact = false;

        impactCoroutine = null;
    }

    private void OnEnable()
    {
        hasImpact = false;
    }

    private void OnDisable()
    {
        if (impactCoroutine != null)
            StopCoroutine(impactCoroutine);
    }
}
