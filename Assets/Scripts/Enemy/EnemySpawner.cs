using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Replaced by the Spawn Manager.")]
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; //list of groups of enemies to be spawned in the wave
        public int waveQuota; //total number of enemies spawned in wave
        public float spawnInterval; //interval that spawns enemies
        public int spawnCount; //enemies that are spawned
    }

    [System.Serializable]
    //reference each specific type of enemy
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; //number of this type in the wave
        public int spawnCount; //number of this type already spawned
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; //list of all waves
    public int currentWaveCount; //count of the current wave

    [Header("Spawner Attributes")]
    float spawnTimer; //used to dertermine when to spawn next enemy
    public int enemiesAlive; //track the spawned enemies alive
    public int maxEnemiesAllowed; //max amount of enemies allowed on the map at one time
    public bool maxEnemiesReached = false; //max number of enemies have been reached
    public float waveInterval; //interval between waves in seconds
    bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; //list to store all spawn positions for enemies


    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        //checks if wave has finished spawning all enemies and begins next wave
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive) 
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        //checks time to spawn the next enemy
        if (spawnTimer > waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        //pauses the execution of the coroutine for a specified amount of time
        yield return new WaitForSeconds(waveInterval);

        //checks if there are more waves after the current wave and move on to the next
        if (currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups) 
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }

    void SpawnEnemies()//will only spawn enemies if it is time for the next wave of enemies to be spawned
    {
        //checks if the current spawn count of enemies is less than the quota
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            //spawns enemies of each type until quota is filled
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                //checks if the minimum number of enimies of this type have spawned
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    //spawns enemy close to the player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    //limit the number of enemies that can be spawned at one time
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    public void OnEnemyKill()
    {
        enemiesAlive--;

        //if the enemies go below the max amount, mark false
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
