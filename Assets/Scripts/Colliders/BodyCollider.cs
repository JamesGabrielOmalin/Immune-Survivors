using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyCollider : MonoBehaviour
{
    private readonly List<IBodyColliderListener> listeners = new();

    // Start is called before the first frame update
    private void Start()
    {
        listeners.AddRange(GetComponents<IBodyColliderListener>());
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var listener in listeners)
        {
            listener.OnBodyColliderEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var listener in listeners)
        {
            listener.OnBodyColliderExit(other);
        }
    }
}

public interface IBodyColliderListener
{
    public abstract void OnBodyColliderEnter(Collider other);
    public abstract void OnBodyColliderExit(Collider other);
}
