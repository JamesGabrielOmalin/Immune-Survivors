using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymptomManager : MonoBehaviour
{
    public static SymptomManager instance;

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
        
    }
}
