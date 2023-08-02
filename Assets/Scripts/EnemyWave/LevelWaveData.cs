using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Wave
{
    public int WaveID = 0;
    public int spawnInterval = 0; // spawn interval in seconds
    public int spawnCounter = 0; //total number of enemies already spawned in this wave
    public int waveQuota = 0; //total number of enemies to spawn in this wave
    public List<EnemyGroup> enemyGroups;

}

[Serializable]

public class EnemyGroup
{
    public string enemyName;
    public int enemyQuota = 0; //total number of this type to spawn 
    public int spawnCounter = 0; //total number of enemies already spawned 
    public int antigenType;
}

[CreateAssetMenu(fileName = "Wave", menuName = "Level/Wave")]
public class LevelWaveData : ScriptableObject
{
    public List<Wave> waveList;
}
