using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private WaveView _waveView;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] _regularEnemyPrefabs; 
    [SerializeField] private GameObject _specialEnemyPrefab;   

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] _zombieSpawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int _waveNumber = 1;
    [SerializeField] private int _spawnMultiplier;
    [SerializeField] private int _specialEnemyStartWave; 
    private float _specialEnemySpawnRate = 0.2f;
    private int _enemyCount;

    public UnityEvent OnAllEnemiesDefeated;

    private void Start()
    {
        SpawnEnemyWave(_waveNumber);
        OnAllEnemiesDefeated.AddListener(HandleWaveCompletion);
    }

    private void SpawnEnemyWave(int waveNumber)
    {
        int totalEnemiesToSpawn = waveNumber + _spawnMultiplier;
        _enemyCount = totalEnemiesToSpawn;

        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            GameObject enemyToSpawn;

            if (waveNumber >= _specialEnemyStartWave && UnityEngine.Random.value < _specialEnemySpawnRate)
            {
                enemyToSpawn = _specialEnemyPrefab;
            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, _regularEnemyPrefabs.Length);
                enemyToSpawn = _regularEnemyPrefabs[randomIndex];
            }

            GameObject enemy = Instantiate(enemyToSpawn, GenerateSpawnPosition(_zombieSpawnPoints).position, Quaternion.identity);
            
            if(enemy.TryGetComponent<AI>(out var enemyAI))
            { 
                enemyAI.OnEnemyDefeated.AddListener(HandleEnemyDefeated);
            }
        }

        _waveView.UpdateWaveVisual(waveNumber);
    }

    private Transform GenerateSpawnPosition(Transform[] spawnPoints)
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }

    private void HandleEnemyDefeated()
    {
        _enemyCount--;

        if (_enemyCount <= 0)
        {
            OnAllEnemiesDefeated.Invoke();
        }
    }

    private void HandleWaveCompletion()
    {
        _waveNumber++;
        SpawnEnemyWave(_waveNumber);
    }

    private void OnDestroy()
    {
        OnAllEnemiesDefeated.RemoveListener(HandleWaveCompletion);
        AI[] enemies = FindObjectsOfType<AI>();
        foreach (var enemy in enemies)
        {
            enemy.OnEnemyDefeated.RemoveListener(HandleEnemyDefeated);
        }
    }
}
