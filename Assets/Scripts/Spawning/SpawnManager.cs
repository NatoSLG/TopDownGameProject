using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex; //The index of the current wave, starting from 0
    int currentWaveSpawnCount; //Tracks how many enemies current wave has spawned

    public WaveData[] data;
    public Camera referenceCamera;

    [Tooltip("If there are more than this number of enemies, stop spawning any more. For Performance.")]
    public int maximumEnemyCount = 300;
    float spawnTimer; //Timer used to determine when to spawn the next group of enemies
    float currentWaveDuration = 0f;

    public static SpawnManager instance;

    void Start()
    {
        if (instance)
        {
            Debug.LogWarning("There is more than 1 Spawn Manager in the scene! Please remove the extras.");
        }
        instance = this;
    }

    void Update()
    {
        //Updates the spawn timer at every frame.
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;

        if (spawnTimer <= 0) 
        {
            //Check if ready to move on to the next wave.
            if (HasWaveEnded())
            {
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;

                //If gone through all the waves, disable this component.
                if (currentWaveIndex >= data.Length) 
                {
                    Debug.Log("All waves have been spawned! Shutting down.", this);
                    enabled = false;
                }

                return;
            }

            //Do not spawn enemies if the conditions are not met.
            if (!CanSpawn())
            {
                spawnTimer += data[currentWaveIndex].GetSpawnInterval();
                return;
            }

            //Get the array of enemies that we are spawning for this tick.
            GameObject[] spawns = data[currentWaveIndex].GetSpawns(EnemyStats.count);

            //Loop through and spawn all the prefabs
            foreach (GameObject prefab in spawns)
            {
                //Stop spawning enemies if limit is exceeded
                if (!CanSpawn())
                {
                    continue;
                }

                //Spawn the enemy
                Instantiate(prefab, GeneratePosition(), Quaternion.identity);
                currentWaveSpawnCount++;
            }

            //Regenerates the spawn timer
            spawnTimer += data[currentWaveIndex].GetSpawnInterval();
        }
    }

    //Check if the requirements are met to continue spawning
    public bool CanSpawn()
    {
        //Don't spawn anymore if max limit is exceeded
        if (HasExceededMaxEnemies())
        {
            return false;
        }

        //Don't spawn if max spawns for the wave is exceeded
        if (instance.currentWaveSpawnCount > instance.data[instance.currentWaveIndex].totalSpawns) 
        {
            return false;
        }

        //Don't spawn if wave's durrection is exceeded
        if (instance.currentWaveDuration > instance.data[instance.currentWaveIndex].duration)
        {
            return false;
        }
        return true;
    }

    //Allows other scripts to check if the maxumum number of enemies has been exceeded
    public static bool HasExceededMaxEnemies()
    {
        //If there is no spawn manager, don't limit max enemies
        if (!instance)
        {
            return false;
        }

        if (EnemyStats.count > instance.maximumEnemyCount)
        {
            return true;
        }
        return false;
    }

    public bool HasWaveEnded()
    {
        WaveData currentWave = data[currentWaveIndex];

        /*If waveDuration is one of the exit conditions, check how long the wave has been running.
         If currentWaveDuration is not greater than wave duration, do not exit yet.*/
        if ((currentWave.exitCondition & WaveData.ExitCondition.waveDuration) > 0)
        {
            if (currentWaveDuration <currentWave.duration) 
            {
                return false;
            }
        }

        /*If reachedTotalSpawns is one of the exit conditions, check if spawned enough
         enemies. If not, return false.*/
        if((currentWave.exitCondition & WaveData.ExitCondition.reachedTotalSpawns) > 0) 
        {
            if (currentWaveSpawnCount < currentWave.totalSpawns) 
            {
                return false;
            }
        }

        //If kill all is checked, make sure there are no more enemies first.
        if (currentWave.mustKillAll && EnemyStats.count > 0)
        {
            return false;
        }
        return true;
    }

    void Reset()
    {
        referenceCamera = Camera.main;
    }

    //Creates a new location where to place the enemy at.
    public static Vector3 GeneratePosition()
    {
        //If there is no reference camera, get one.
        if (!instance.referenceCamera)
        {
            instance.referenceCamera = Camera.main;
        }

        //Give a warning if the gamera is not orthographic
        if (!instance.referenceCamera.orthographic) 
        {
            Debug.LogWarning("The reference camera is not orthographic!" +
                "This will cause enemy spawns to sometimes sppear within camera boundaries!");
        }

        //Generate a position outside of camera boundaries using 2 random numbers.
        float x = Random.Range(0f, 1f);
        float y = Random.Range(0f, 1f);

        //Randomly choose whether to round the x or the y value.
        switch(Random.Range(0, 2)) 
        {
            case 0: default:
                return instance.referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x), y));
            case 1:
                return instance.referenceCamera.ViewportToWorldPoint(new Vector3(x, Mathf.Round(y)));
        }
    }

    //Checking if the enemy is withing the camera's boundaries.
    public static bool IsWithinBoundaries(Transform checkedObject)
    {
        //Get the camera to check if withing boundaries
        Camera c = instance && instance.referenceCamera ? instance.referenceCamera : Camera.main;

        Vector2 viewport = c.WorldToViewportPoint(checkedObject.position);

        if (viewport.x < 0f || viewport.x > 1f)
        {
            return false;
        }

        if (viewport.y < 0f || viewport.y > 1f) 
        {
            return false;
        }
        return true;
    }
}
