using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;

    // Start is called before the first frame update
    private void Start()
    {
        if (!rigidBody)
            rigidBody = GetComponent<Rigidbody>();
    }

    public void AddImpact(Vector3 dir, float force)
    {
        if (!rigidBody)
            return;

        dir.Normalize();
        rigidBody.AddForce(dir * force, ForceMode.VelocityChange);
    }
}
