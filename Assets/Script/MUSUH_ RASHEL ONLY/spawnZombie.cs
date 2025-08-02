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
    public Image waveProgressBar; // Changed from Slider to Image
    public TextMeshProUGUI finalWaveText;

    [Header("Progress Type")]
    [SerializeField] private bool useOverallProgress = true; // Toggle untuk memilih mode

    [Header("Audio")]
    public AudioSource waveAudio;
    public AudioClip warningClip;
    [Header("Wave Warning System")]
    public GameObject warningPanel; // Panel container untuk warning
    public Image warningImage; // Warning icon/image
    public TextMeshProUGUI warningText; // Text "INCOMING WAVE" etc
    public float warningDuration = 3f; // Durasi warning muncul

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    // Variables untuk overall progress
    private int totalEnemiesAllWaves = 0;
    private int spawnedEnemiesAllWaves = 0;

    private void Awake()
    {
        // Hide warning panel at start
        if (warningPanel != null)
            warningPanel.SetActive(false);
        if (targetTower == null)
            targetTower = GameObject.FindWithTag("PlayerBase")?.transform;

        if (finalWaveText != null)
            finalWaveText.gameObject.SetActive(false);

        // Hitung total enemies dari semua wave
        CalculateTotalEnemies();
    }

    private void CalculateTotalEnemies()
    {
        totalEnemiesAllWaves = 0;
        foreach (Wave wave in waves)
        {
            foreach (WaveSettings ws in wave.WaveSettings)
            {
                totalEnemiesAllWaves += ws.EnemyCount;
            }
        }
        Debug.Log($"[WaveSpawner] Total enemies across all waves: {totalEnemiesAllWaves}");
    }

    private void Start()
    {
        // Initialize progress bar
        if (waveProgressBar != null)
        {
            waveProgressBar.fillAmount = 0f;
            waveProgressBar.type = Image.Type.Filled; // Ensure it's set to Filled
        }

        StartCoroutine(HandleWaveFlow());
    }

    private IEnumerator HandleWaveFlow()
    {
        while (currentWaveIndex < waves.Length)
        {
            Debug.Log($"[WaveSpawner] Starting wave {currentWaveIndex + 1}");
            if (currentWaveIndex > 0)
            {
                yield return StartCoroutine(ShowWaveWarning());
            }

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

        // Set progress bar to 100% when all waves complete
        if (waveProgressBar != null)
            waveProgressBar.fillAmount = 1f;
    }

    private IEnumerator ShowWaveWarning()
    {
        if (warningPanel == null) yield break;

        // Determine warning message based on wave
        string warningMessage = "";
        if (currentWaveIndex == waves.Length - 1)
        {
            warningMessage = "FINAL WAVE INCOMING!";
        }
        else
        {
            warningMessage = $"WAVE {currentWaveIndex + 1} INCOMING!";
        }

        // Set warning text
        if (warningText != null)
            warningText.text = warningMessage;

        // Show warning panel
        warningPanel.SetActive(true);
        if (warningClip != null && waveAudio != null)
        {
            waveAudio.clip = warningClip;
            waveAudio.loop = true;
            waveAudio.Play();
        }

        // Keep warning visible for specified duration
        yield return new WaitForSeconds(warningDuration);

        // Hide warning panel
        warningPanel.SetActive(false);
        waveAudio.Stop();

        Debug.Log($"<color=yellow>[WaveSpawner] {warningMessage}</color>");
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

        // Setup progress tracking based on mode
        int totalEnemiesThisWave = 0;
        int spawnedEnemiesThisWave = 0;

        if (!useOverallProgress)
        {
            foreach (var ws in pending)
                totalEnemiesThisWave += ws.EnemyCount;

            if (waveProgressBar != null)
                waveProgressBar.fillAmount = 0f; // Reset for individual wave
        }

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

                    // Update counters
                    spawnedEnemiesThisWave++;
                    spawnedEnemiesAllWaves++;

                    // Update progress bar based on selected mode
                    if (waveProgressBar != null)
                    {
                        if (useOverallProgress && totalEnemiesAllWaves > 0)
                        {
                            // Overall progress across all waves
                            waveProgressBar.fillAmount = (float)spawnedEnemiesAllWaves / totalEnemiesAllWaves;
                        }
                        else if (!useOverallProgress && totalEnemiesThisWave > 0)
                        {
                            // Individual wave progress (original behavior)
                            waveProgressBar.fillAmount = (float)spawnedEnemiesThisWave / totalEnemiesThisWave;
                        }
                    }

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

    // Method untuk mengubah mode progress secara runtime
    public void SetOverallProgressMode(bool enabled)
    {
        useOverallProgress = enabled;

        if (waveProgressBar != null)
        {
            if (useOverallProgress && totalEnemiesAllWaves > 0)
            {
                waveProgressBar.fillAmount = (float)spawnedEnemiesAllWaves / totalEnemiesAllWaves;
            }
            else
            {
                // Reset untuk mode individual wave
                waveProgressBar.fillAmount = 0f;
            }
        }
    }

    // Method untuk debug info
    public void GetProgressInfo()
    {
        Debug.Log($"Current Wave: {currentWaveIndex + 1}/{waves.Length}");
        Debug.Log($"Spawned This Session: {spawnedEnemiesAllWaves}/{totalEnemiesAllWaves}");
        Debug.Log($"Overall Progress: {((float)spawnedEnemiesAllWaves / totalEnemiesAllWaves * 100f):F1}%");
    }
}