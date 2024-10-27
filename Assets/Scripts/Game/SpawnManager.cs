using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _foxPrefab;

    [SerializeField] private int _enemyCount;
    [SerializeField] private int _waveNumber = 1;

    [SerializeField] private Transform[] _zombieSpawnPoints;
    [SerializeField] private Transform[] _foxSpawnPoints;

    private void Start()
    {
        SpawnEnemyWave(_waveNumber);
    }
    private void Update()
    {
        _enemyCount = FindObjectsOfType<Enemy>().Length;
        if (_enemyCount == 0)
        {
            _waveNumber++;
            SpawnEnemyWave(_waveNumber);
        }
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(_enemyPrefab, GenerateSpawnPosition(_zombieSpawnPoints).position, Quaternion.identity);
            Instantiate(_foxPrefab, GenerateSpawnPosition(_foxSpawnPoints).position, Quaternion.identity);
        }
    }

    private Transform GenerateSpawnPosition(Transform[] spawnPoints)
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform randomSpawnPos = _zombieSpawnPoints[randomIndex];
        return randomSpawnPos;
    }
}
