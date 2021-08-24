using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : Switch
{
    public Switch switchToTriggerWaves;
    public List<Wave> waves;
    public float pauseTimeBetweenWaves;
    public Door backDoorToClose;
    public Sound nextWaveSound;

    [Header("Testing options")]
    public int skipToWave;

    private bool wavesAreUnfolding;
    private int currentWaveIndex;
    private bool waveSpawned;
    private float pauseTimeElapsed;

    private bool allEnnemiesKilled;
    private bool lateStartFlag;

    private void Awake()
    {
        lateStartFlag = true;
    }

    private new void Start()
    {
        base.Start();
    }

    private void LateStart()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            for (int y = 0; y < waves[i].waveEnemies.Count; y++)
            {
                waves[i].waveEnemies[y].Deactivate();
            }
        }
        if(backDoorToClose != null)
            backDoorToClose.Open();
    }

    private void Update()
    {
        if(lateStartFlag)
        {
            lateStartFlag = false;
            LateStart();
        }
        else
        {
            if (switchToTriggerWaves != null && switchToTriggerWaves.IsON() && !wavesAreUnfolding && !isOn)
            {
                StartWaves();
            }

            if (wavesAreUnfolding)
            {
                if (currentWaveIndex < waves.Count)
                {
                    if (!waveSpawned)
                    {
                        waveSpawned = true;
                        for (int i = 0; i < waves[currentWaveIndex].waveEnemies.Count; i++)
                        {
                            StartCoroutine(waves[currentWaveIndex].waveEnemies[i].Activate());
                            waves[currentWaveIndex].waveEnemies[i].provoked = true;
                        }

                        for (int i = 0; i < waves[currentWaveIndex].objectToDisable.Count; i++)
                        {
                            waves[currentWaveIndex].objectToDisable[i].SetActive(false);
                        }

                        for (int i = 0; i < waves[currentWaveIndex].objectToEnable.Count; i++)
                        {
                            waves[currentWaveIndex].objectToEnable[i].SetActive(true);
                        }

                        for (int i = 0; i < waves[currentWaveIndex].switchesToDisable.Count; i++)
                        {
                            waves[currentWaveIndex].switchesToDisable[i].isOn = false;
                        }

                        for (int i = 0; i < waves[currentWaveIndex].switchesToEnable.Count; i++)
                        {
                            waves[currentWaveIndex].switchesToEnable[i].isOn = true;
                        }

                        if(nextWaveSound.clip != null)
                            GameData.playerSource.PlayOneShot(nextWaveSound.clip, nextWaveSound.volumeScale);

                        GameData.playerManager.Heal(waves[currentWaveIndex].hpRestored);

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
                            if (pauseTimeElapsed < pauseTimeBetweenWaves)
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
                    if (backDoorToClose != null && !backDoorToClose.isOpened)
                        backDoorToClose.Open();
                    isOn = true;
                }
            }
        }
    }

    private void StartWaves()
    {
        if(skipToWave - 1 > 0)
        {
            currentWaveIndex = skipToWave - 1;
        }
        else
        {
            currentWaveIndex = 0;
        }
        wavesAreUnfolding = true;
        waveSpawned = false;
        if (backDoorToClose != null)
            backDoorToClose.Close();
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }

    [System.Serializable]
    public class Wave
    {
        public List<Enemy> waveEnemies;
        public int hpRestored;
        public List<GameObject> objectToEnable;
        public List<GameObject> objectToDisable;
        public List<LinkSwitch> switchesToEnable;
        public List<LinkSwitch> switchesToDisable;
    }
}
