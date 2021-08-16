using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : Switch
{
    public Switch switchToTriggerWaves;
    public List<Wave> waves;
    public float pauseTimeBetweenWaves;

    private bool wavesAreUnfolding;
    private int currentWaveIndex;
    private bool waveSpawned;
    private float pauseTimeElapsed;

    private bool allEnnemiesKilled;

    private new void Start()
    {
        base.Start();
        for (int i = 0; i < waves.Count; i++)
        {
            for (int y = 0; y < waves[i].waveEnemies.Count; y++)
            {
                waves[i].waveEnemies[y].Deactivate();
            }
        }
    }

    private void Update()
    {
        if (switchToTriggerWaves != null && switchToTriggerWaves.IsON() && !wavesAreUnfolding && !isOn)
        {
            StartWaves();
        }

        if(wavesAreUnfolding)
        {
            if(currentWaveIndex < waves.Count)
            {
                if (!waveSpawned)
                {
                    waveSpawned = true;
                    for (int i = 0; i < waves[currentWaveIndex].waveEnemies.Count; i++)
                    {
                        waves[currentWaveIndex].waveEnemies[i].Activate();
                        waves[currentWaveIndex].waveEnemies[i].provoked = true;
                    }
                    pauseTimeElapsed = 0;
                }
                else
                {
                    allEnnemiesKilled = true;
                    for (int i = 0; i < waves[currentWaveIndex].waveEnemies.Count; i++)
                    {
                        if (waves[currentWaveIndex].waveEnemies[i] != null && !waves[currentWaveIndex].waveEnemies[i].isDying)
                        {
                            allEnnemiesKilled = false;
                        }
                    }

                    if (allEnnemiesKilled)
                    {
                        if(pauseTimeElapsed < pauseTimeBetweenWaves)
                        {
                            pauseTimeElapsed += Time.deltaTime;
                        }
                        else
                        {
                            currentWaveIndex++;
                            waveSpawned = false;
                        }
                    }
                }
            }
            else
            {
                wavesAreUnfolding = false;
                isOn = true;
            }
        }
    }

    private void StartWaves()
    {
        currentWaveIndex = 0;
        wavesAreUnfolding = true;
        waveSpawned = false;
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }

    [System.Serializable]
    public class Wave
    {
        public List<Enemy> waveEnemies;
    }
}
