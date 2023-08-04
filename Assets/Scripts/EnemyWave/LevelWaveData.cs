using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[Serializable]
public class Wave
{
    [Header ("Wave")]
    public int WaveID = 0;
    public int waveSpawnQuota = 0; //total number of enemies to spawn in this wave
    
    [Header("Spawning")]
    public float spawnInterval = 0; // spawn interval in seconds
    public int spawnCounter = 0; //total number of enemies already spawned in this wave
    public int excessSpawnCounter = 0; //total number of enemies already spawned in this wave


    [Header("Spawnables")]
    public List<EnemyGroup> enemyGroups;

}

[Serializable]

public class EnemyGroup
{
    public string enemyName;
    public int antigenType;
    public int spawnCounter = 0; //total number of enemies already spawned 
    public int enemyQuota = 0; //total number of this type to spawn 
}

[CreateAssetMenu(fileName = "Wave", menuName = "Level/Wave")]
public class LevelWaveData : ScriptableObject
{
    public int waveInterval = 0;
    public List<Wave> waveList;
}
