using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnZombie : MonoBehaviour
{
    [System.Serializable]
    public class WaveSettings
    {
        public GameObject Enemy;              // Prefab zombie
        public Transform NeededSpawner;       // Lokasi spawner
        public int EnemyCount = 1;            // Jumlah zombie
        public float SpawnDelay = 1f;         // Delay antar spawn
    }

    [System.Serializable]
    public class Wave
    {
        public WaveSettings[] WaveSettings;
    }

    [Header("Wave Configuration")]
    [SerializeField] private Wave[] waves;

    [Header("Target References")]
    [SerializeField] private Transform targetTower;

    private int currentWaveIndex;
    private bool isSpawning;
    private bool waitingForNextWave;

    private void Awake()
    {
        if (targetTower == null)
            targetTower = GameObject.FindWithTag("PlayerBase")?.transform;
    }

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            Debug.Log($"[WaveSpawner] Starting wave {currentWaveIndex + 1}");

            isSpawning = true;
            waitingForNextWave = false;

            StartCoroutine(SpawnEnemyInWave());
        }
        else
        {
            Debug.Log("[WaveSpawner] All waves completed!");
        }
    }

    private IEnumerator SpawnEnemyInWave()
    {
        Wave currentWave = waves[currentWaveIndex];

        foreach (WaveSettings setting in currentWave.WaveSettings)
        {
            for (int i = 0; i < setting.EnemyCount; i++)
            {
                Vector3 spawnPos = setting.NeededSpawner.position;
                spawnPos.y = 0.2f;

                GameObject newEnemy = Instantiate(setting.Enemy, spawnPos, Quaternion.identity);

                ZombieEnemy zombie = newEnemy.GetComponent<ZombieEnemy>();
                ZombieFast zombieFast = newEnemy.GetComponent<ZombieFast>();
                if (zombie != null && targetTower != null)
                {
                    zombie.SetTargetTower(targetTower);
                }
                if (zombieFast != null && targetTower != null)
                {
                    zombieFast.SetTargetTower(targetTower);
                }

                yield return new WaitForSeconds(setting.SpawnDelay);
            }
        }

        isSpawning = false;
        waitingForNextWave = true;

        StartCoroutine(WaitUntilAllEnemiesDefeated());
    }

    private IEnumerator WaitUntilAllEnemiesDefeated()
    {
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        currentWaveIndex++;
        StartNextWave();
    }

    public void LaunchWave()
    {
        if (!isSpawning && !waitingForNextWave)
        {
            StartNextWave();
        }
    }
}
