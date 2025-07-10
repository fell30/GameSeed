using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private Wave[] _waves;
    [SerializeField] private Transform enemyTarget;

    private int _currentEnemyIndex;
    private int _currentWaveIndex;
    private int _enemiesLeftToSpawn;

    private bool _isSpawning;
    private bool _waitingForNextWave;

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (_currentWaveIndex < _waves.Length)
        {
            Debug.Log($"[WaveSpawner] Starting wave {_currentWaveIndex + 1}");

            _currentEnemyIndex = 0;
            _enemiesLeftToSpawn = _waves[_currentWaveIndex].WaveSettings.Length;

            _isSpawning = true;
            _waitingForNextWave = false;

            StartCoroutine(SpawnEnemyInWave());
        }
        else
        {
            Debug.Log("[WaveSpawner] All waves completed!");
        }
    }

    private IEnumerator SpawnEnemyInWave()
    {
        while (_enemiesLeftToSpawn > 0)
        {
            WaveSettings setting = _waves[_currentWaveIndex].WaveSettings[_currentEnemyIndex];

            yield return new WaitForSeconds(setting.SpawnDelay);

            Vector3 spawnPos = setting.NeededSpawner.transform.position;
            spawnPos.y = 0.2f; 

            GameObject newEnemy = Instantiate(setting.Enemy, spawnPos, Quaternion.identity);


            EnemyNavAI ai = newEnemy.GetComponent<EnemyNavAI>();
            if (ai != null && enemyTarget != null)
            {
                ai.SetTarget(enemyTarget);
            }

            _currentEnemyIndex++;
            _enemiesLeftToSpawn--;
        }

        _isSpawning = false;
        _waitingForNextWave = true;

        StartCoroutine(WaitUntilAllEnemiesDefeated());
    }

    private IEnumerator WaitUntilAllEnemiesDefeated()
    {
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        _currentWaveIndex++;
        StartNextWave();
    }

    public void LaunchWave()
    {
        if (!_isSpawning && !_waitingForNextWave)
        {
            StartNextWave();
        }
    }
}
