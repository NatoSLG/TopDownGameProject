using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;//stores the prefabs of the different terrain chunks
    public GameObject player; //references player
    public float checkerRadius;
    public LayerMask terrainMask; //Determins which layer is and is not the terrain
    public GameObject currentChunk;
    Vector3 playerLastPosition;

    //allows chunks to despawn while player is not near them
    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDistance; //Must be greater than the L and W of the tilemap
    float opDistance; //Determins the distance of each chunk from the player
    float optimizerCooldown;
    public float optimizerCooldownDuration;

    // Start is called before the first frame update
    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition; //players movement direction
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(directionName);
        
        //checks adjacent directions for diagonal chunks
        if (directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
            CheckAndSpawnChunk("Left Up");
            CheckAndSpawnChunk("Right Up");
        }
        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
            CheckAndSpawnChunk("Left Down");
            CheckAndSpawnChunk("Right Down");
        }
        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
            CheckAndSpawnChunk("Right Up");
            CheckAndSpawnChunk("Right Down");
        }
        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
            CheckAndSpawnChunk("Left Up");
            CheckAndSpawnChunk("Left Down");
        }
    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    //determins the name representing the direction
    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        //determins if movement is primarily horizontal or vertical
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            //moving horizontally more than vertically
            if(direction.y > 0.5f)
            {
                //also moving up
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5f)
            {
                //also moving down
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                //moving straight horizontally
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            //moving vertically more than horizontally
            if (direction.x > 0.5f)
            {
                //also moving right
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5f)
            {
                //also moving left
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                //moving straight vertically
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);

    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;//creates a countdown loop for the despawning of chunks
        if (optimizerCooldown < 0f)
        {
            optimizerCooldown = optimizerCooldownDuration;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDistance = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDistance > maxOpDistance)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
