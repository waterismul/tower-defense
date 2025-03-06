using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves;//현재 스테이지의 모든 웨이브 정보
    [SerializeField] private EnemySpawner enemySpawner;
    private int currentWaveIndex = -1;//현재 웨이브 인덱스
    
    //웨이브 정보 출력을 위한 프로퍼티
    public int CurrentWave=>currentWaveIndex+1;//시작이 0이라서
    public int MaxWave => waves.Length;


    public void StartWave()
    {
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length - 1)
        {
            currentWaveIndex++;//인덱스의 초기값이 -1이기 때문에 웨이브 인덱스 증가를 먼저함
            enemySpawner.StartWave(waves[currentWaveIndex]);//웨이브 정보 제공
        }
    }

}

[Serializable]
public struct Wave
{
    public float spawnTime;//현재 웨이브 적 생성 주기
    public int maxEnemyCount;//현재 웨이브 적 등장 숫자
    public GameObject[] enemyPrefabs;//현재 웨이브 적 등장 종류
}
