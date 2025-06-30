using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Wave[] _waves;

    private int _currentEnemyIndex;
    private int _currentWaveIndex;
    private int _enemiesLeftToSpawn;
    private bool _isSpawning;

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (_currentWaveIndex < _waves.Length)
        {
            _currentEnemyIndex = 0;
            _enemiesLeftToSpawn = _waves[_currentWaveIndex].WaveSettings.Length;
            if (!_isSpawning)
            {
                StartCoroutine(SpawnEnemyInWave());
            }
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    private IEnumerator SpawnEnemyInWave()
    {
        _isSpawning = true;

        while (_enemiesLeftToSpawn > 0)
        {
            WaveSettings setting = _waves[_currentWaveIndex].WaveSettings[_currentEnemyIndex];

            yield return new WaitForSeconds(setting.SpawnDelay);

            Instantiate(setting.Enemy, setting.NeededSpawner.transform.position, Quaternion.identity);

            _enemiesLeftToSpawn--;
            _currentEnemyIndex++;
        }

        _currentWaveIndex++;
        _isSpawning = false;
        StartNextWave();
    }

    public void LaunchWave()
    {
        if (!_isSpawning)
        {
            StartNextWave();
        }
    }
}

[System.Serializable]
public class Wave
{
    [SerializeField] private WaveSettings[] waveSettings;
    public WaveSettings[] WaveSettings => waveSettings;
}

[System.Serializable]
public class WaveSettings
{
    [SerializeField] private GameObject _enemy;
    public GameObject Enemy => _enemy;

    [SerializeField] private GameObject _neededSpawner;
    public GameObject NeededSpawner => _neededSpawner;

    [SerializeField] private float _spawnDelay;
    public float SpawnDelay => _spawnDelay;
}
