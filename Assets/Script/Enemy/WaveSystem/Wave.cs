using System;
using UnityEngine;

[Serializable]
public class Wave
{
    [SerializeField] private WaveSettings[] waveSettings;
    public WaveSettings[] WaveSettings => waveSettings;
}

[Serializable]
public class WaveSettings
{
    [SerializeField] private GameObject _enemy;
    public GameObject Enemy => _enemy;

    [SerializeField] private GameObject _neededSpawner;
    public GameObject NeededSpawner => _neededSpawner;

    [SerializeField] private float _spawnDelay = 1f;
    public float SpawnDelay => _spawnDelay;
}
