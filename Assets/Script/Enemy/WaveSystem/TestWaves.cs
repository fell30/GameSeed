using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaves : MonoBehaviour
{
    private WaveSpawner _waveSpawner;

    private void Awake()
    {
        GameObject spawnerObject = GameObject.Find("Wave Spawner");
        if (spawnerObject != null)
        {
            _waveSpawner = spawnerObject.GetComponent<WaveSpawner>();
        }
        else
        {
            Debug.LogError("Wave Spawner not found in the scene!");
        }
    }

    private void OnDestroy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Hanya lanjut jika tidak ada enemy tersisa
        if (enemies.Length == 0 && _waveSpawner != null)
        {
            _waveSpawner.LaunchWave();
        }
    }
}
