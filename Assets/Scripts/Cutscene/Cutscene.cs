using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private AudioSource BGM;

    public void Skip(float time)
    {
        BGM.time = time;
        director.time = time;
    }
}
