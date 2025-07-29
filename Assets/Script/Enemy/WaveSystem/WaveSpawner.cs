using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private Wave[] _waves;

    [Header("Target References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemyTarget; // Misalnya: PlayerBase

    private int _currentEnemyIndex;
    private int _currentWaveIndex;
    private int _enemiesLeftToSpawn;

    private bool _isSpawning;
    private bool _waitingForNextWave;

    private void Awake()
    {
        // Jika player atau enemyTarget belum di-assign di Inspector, coba cari otomatis
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (enemyTarget == null)
            enemyTarget = GameObject.FindWithTag("PlayerBase")?.transform;
    }

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
            spawnPos.y = 0.2f; // agar muncul di atas permukaan

            GameObject newEnemy = Instantiate(setting.Enemy, spawnPos, Quaternion.identity);

            // Setup FSM enemy
            Enemy fsmEnemy = newEnemy.GetComponent<Enemy>();
            if (fsmEnemy != null)
            {
                fsmEnemy.SetTargets(player, enemyTarget);
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
