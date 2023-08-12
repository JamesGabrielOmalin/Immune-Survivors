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
    
    [Header("Spawning")]
    public float spawnInterval = 0; // spawn interval in seconds

    [Header("Spawnables")]
    [Tooltip("Types of enemies to spawn")]
    public List<EnemyGroup> enemyGroups;

    public List<BossEnemyGroup> BossEnemyGroups;

    [Header("Tracker [DO NOT MODIFY]")]
    public int waveSpawnCounter = 0; //total number of enemies already spawned in this wave
    [Tooltip("Target number of enemies in this wave to spawn")]
    public int waveSpawnQuota = 0; //total number of enemies to spawn in this wave

}

[Serializable]

public class EnemyGroup
{
    public string enemyName;
    [Tooltip("Enemy type based on its antigen type")]
    public int antigenType;

    [Tooltip("Target number of this type to spawn")]
    public int enemyQuota = 0;

    [Header("Tracker [DO NOT MODIFY]")]
    public int enemySpawnCounter = 0; //total number of enemies already spawned 

}

[Serializable]
public class BossEnemyGroup
{
    public string enemyName;
    [Tooltip("Enemy type based on its antigen type")]
    public int antigenType;

    [Tooltip("Number of this type to spawn")]
    public int count = 0;

    [Header("Tracker [DO NOT MODIFY]")]
    public int bossSpawnCounter = 0; //total number of enemies already spawned 

}

[CreateAssetMenu(fileName = "Wave", menuName = "Level/Wave")]
public class LevelWaveData : ScriptableObject
{
    public int maxActiveEnemyThreshold = 0;
    public int waveInterval = 0;
    public List<Wave> waveList;
}
