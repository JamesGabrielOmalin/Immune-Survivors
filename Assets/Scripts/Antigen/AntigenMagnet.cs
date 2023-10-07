using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class AntigenMagnet : MonoBehaviour, IBodyColliderListener
{
    [SerializeField] private float magnetSpeed;

    public void OnBodyColliderEnter(Collider other)
    {
        if (other.TryGetComponent<Antigen>(out Antigen antigen))
        {
            StartCoroutine(MagnetizeAntigen(antigen));
        }
    }

    public void OnBodyColliderExit(Collider other)
    {

    }

    private IEnumerator MagnetizeAntigen(Antigen antigen)
    {
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime * magnetSpeed;

            antigen.transform.position = Vector3.Lerp(antigen.transform.position, this.transform.position, t);
            yield return null;
        }
        AudioManager.instance.Play("PlayerPickUp", transform.position);

        AntigenManager.instance.AddAntigen(antigen.Type);
        AntigenManager.instance.OnAntigenPickup?.Invoke();
        antigen.gameObject.SetActive(false);
        yield break;
    }
}
