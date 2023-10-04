using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoughPingController : MonoBehaviour
{
    public static CoughPingController instance;
    [SerializeField] List<GameObject> pingList;
    Dictionary<string, GameObject> pingDictionary;
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
        pingDictionary = new Dictionary<string, GameObject>()
        {
            {"LEFT",pingList[0]},
            {"RIGHT",pingList[1]},
            {"TOP",pingList[2]},
            {"BOTTOM",pingList[3]}
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ActivatePing(string direction,float delay)
    {
        Debug.Log("Direction: " + direction);
        pingDictionary[direction].SetActive(true);

        yield return new WaitForSeconds(delay);

        pingDictionary[direction].SetActive(false);
    }
}
