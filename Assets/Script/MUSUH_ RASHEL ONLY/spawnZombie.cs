using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class spawnZombie : MonoBehaviour
{
    [System.Serializable]
    public class WaveSettings
    {
        public GameObject Enemy;
        public Transform[] PossibleSpawners;
        public int EnemyCount = 1;
        public float SpawnDelay = 1f;
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

    [Header("Wave Timing")]
    public float timeBetweenWaves = 10f;

    [Header("UI Elements")]
    public Slider waveProgressBar;
    public TextMeshProUGUI finalWaveText;

    [Header("Audio")]
    public AudioSource waveAudio;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    private void Awake()
    {
        if (targetTower == null)
            targetTower = GameObject.FindWithTag("PlayerBase")?.transform;

        if (finalWaveText != null)
            finalWaveText.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(HandleWaveFlow());
    }

    private IEnumerator HandleWaveFlow()
    {
        while (currentWaveIndex < waves.Length)
        {
            Debug.Log($"[WaveSpawner] Starting wave {currentWaveIndex + 1}");

            if (currentWaveIndex == waves.Length - 1)
            {
                Debug.Log("<color=red>[WaveSpawner] A HUGE WAVE OF ZOMBIES IS APPROACHING!</color>");

                if (finalWaveText != null)
                    finalWaveText.gameObject.SetActive(true);

                if (waveAudio != null)
                    waveAudio.Play();

                yield return new WaitForSeconds(2.5f);

                if (finalWaveText != null)
                    finalWaveText.gameObject.SetActive(false);
            }

            yield return StartCoroutine(SpawnCurrentWave());
            yield return StartCoroutine(WaitUntilAllEnemiesDefeated());

            currentWaveIndex++;

            if (currentWaveIndex < waves.Length)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("<color=green>[WaveSpawner] All waves complete!</color>");
    }

    private IEnumerator SpawnCurrentWave()
    {
        isSpawning = true;

        Wave currentWave = waves[currentWaveIndex];
        List<WaveSettings> pending = new List<WaveSettings>();

        // Clone settings so we can modify counts
        foreach (var ws in currentWave.WaveSettings)
        {
            WaveSettings clone = new WaveSettings()
            {
                Enemy = ws.Enemy,
                PossibleSpawners = ws.PossibleSpawners,
                EnemyCount = ws.EnemyCount,
                SpawnDelay = ws.SpawnDelay
            };
            pending.Add(clone);
        }

        // Total and spawn tracking for progress bar
        int totalEnemies = 0;
        foreach (var ws in pending)
            totalEnemies += ws.EnemyCount;

        int spawnedEnemies = 0;
        if (waveProgressBar != null)
            waveProgressBar.value = 0f;

        while (pending.Count > 0)
        {
            for (int i = pending.Count - 1; i >= 0; i--)
            {
                WaveSettings ws = pending[i];

                if (ws.EnemyCount > 0)
                {
                    Transform spawnPoint = ws.PossibleSpawners[Random.Range(0, ws.PossibleSpawners.Length)];
                    Vector3 spawnPos = spawnPoint.position;
                    spawnPos.y = 0.2f;

                    GameObject newEnemy = Instantiate(ws.Enemy, spawnPos, Quaternion.identity);

                    // Assign target
                    var zombie = newEnemy.GetComponent<ZombieEnemy>();
                    var zombieFast = newEnemy.GetComponent<ZombieFast>();
                    if (zombie != null) zombie.SetTargetTower(targetTower);
                    if (zombieFast != null) zombieFast.SetTargetTower(targetTower);

                    ws.EnemyCount--;
                    spawnedEnemies++;

                    // Update progress bar
                    if (waveProgressBar != null && totalEnemies > 0)
                        waveProgressBar.value = (float)spawnedEnemies / totalEnemies;

                    yield return new WaitForSeconds(ws.SpawnDelay);
                }
                else
                {
                    pending.RemoveAt(i);
                }
            }
        }

        isSpawning = false;
    }

    private IEnumerator WaitUntilAllEnemiesDefeated()
    {
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
