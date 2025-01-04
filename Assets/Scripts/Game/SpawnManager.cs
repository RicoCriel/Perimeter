using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private WaveView _waveView;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _enemies;

    [SerializeField] private int _enemyCount;
    [SerializeField] private int _waveNumber = 1;
    private const int _enemyNumber = 10;

    [SerializeField] private Transform[] _zombieSpawnPoints;
    

    private void Start()
    {
        SpawnEnemyWave(_waveNumber + _enemyNumber);
    }
    private void Update()
    {
        _enemyCount = FindObjectsOfType<Enemy>().Length;
        if (_enemyCount == 0)
        {
            _waveNumber++;
            SpawnEnemyWave(_waveNumber + _enemyNumber);
        }
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(_enemyPrefab, GenerateSpawnPosition(_zombieSpawnPoints).position, Quaternion.identity);
        }
        _waveView.UpdateWaveVisual(_waveNumber);
    }

    private Transform GenerateSpawnPosition(Transform[] spawnPoints)
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform randomSpawnPos = _zombieSpawnPoints[randomIndex];
        return randomSpawnPos;
    }

}
