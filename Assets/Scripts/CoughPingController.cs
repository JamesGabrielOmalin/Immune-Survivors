using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoughPingController : MonoBehaviour
{
    public static CoughPingController instance;
    [SerializeField] List<GameObject> pingList;
    Dictionary<SymptomEffect.KnockbackDirection, GameObject> pingDictionary;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pingDictionary = new Dictionary<SymptomEffect.KnockbackDirection, GameObject>()
        {
            {SymptomEffect.KnockbackDirection.Left,pingList[0]},
            {SymptomEffect.KnockbackDirection.Right,pingList[1]},
            {SymptomEffect.KnockbackDirection.Top,pingList[2]},
            {SymptomEffect.KnockbackDirection.Bottom,pingList[3]}
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ActivatePing(SymptomEffect.KnockbackDirection direction, float duration)
    {
        StartCoroutine(PingCoroutine(direction, duration));
    }
    public IEnumerator PingCoroutine(SymptomEffect.KnockbackDirection direction,float duration)
    {
        Debug.Log("Direction: " + direction);
        pingDictionary[direction].SetActive(true);

        yield return new WaitForSeconds(duration);

        pingDictionary[direction].SetActive(false);
    }
}
