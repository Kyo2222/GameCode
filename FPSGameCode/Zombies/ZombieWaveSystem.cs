using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieWaveSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject[] zombiePrefabs;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private float timeBetweenWaves = 10f;
    [SerializeField] private float waveTimer = 0f;
    private int waveNumber = 1;
    [SerializeField]
    private int zombiePerWave = 4;
    
    [Header("UI")]
    [SerializeField]
    private Text WaveNumber;
    [SerializeField]
    private Text WaveTimer;

    private void Update()
    {
        LoadSetting();
        
        if(waveNumber == 10)
            return;

        waveTimer += Time.deltaTime;

        int intValue = Mathf.RoundToInt(waveTimer);
        WaveTimer.text = intValue.ToString();

        if(waveTimer >= timeBetweenWaves)
        {
            StartNewWave();
        }
    }

    private void StartNewWave()
    {
        waveTimer = 0f;

        zombiePerWave += 2;

        var minDistance = 4f;

        for(int i = 0; i < zombiePerWave; i++)
        {
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            var spawnPoint = spawnPoints[randomSpawnIndex];

            var randomZombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];

            var spawnPosition = spawnPoint.position + Random.insideUnitSphere * minDistance;

            spawnPosition.y = spawnPoint.position.y;

            Instantiate(randomZombiePrefab, spawnPosition, spawnPoint.rotation);
        }

        waveNumber++;
        WaveNumber.text = waveNumber.ToString();
    }

    private void LoadSetting()
    {
        if(PlayerPrefs.HasKey("TimeBetweenWaves"))
        {
            timeBetweenWaves = PlayerPrefs.GetFloat("TimeBetweenWaves");
        }

        if(PlayerPrefs.HasKey("ZombiesPerWave"))
        {
            zombiePerWave = PlayerPrefs.GetInt("ZombiesPerWave");
        }
    }
}
