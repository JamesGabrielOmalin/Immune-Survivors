using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMovement : MonoBehaviour
{
    [SerializeField] private float rotationRate;
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up, rotationRate * Time.fixedDeltaTime);
    }
}
