using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    protected EnemyStats enemy;
    protected Transform player;/*keeps track of players position*/
    SpriteRenderer sr;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction
    {
        none,
        respawnAtEdge,
        despawn
    }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;

    protected bool spawnedOutOfFrame = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
        enemy = GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();

        //Picks a random player on the screen, instead of always picking the 1st player.
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        player = allPlayers[Random.Range(0, allPlayers.Length)].transform;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //if we are being knockbacked, process the knockback
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleOutOfFrameAction();
        }
    }

    //If the enemy falls outside of the frame, handle it.
    protected virtual void HandleOutOfFrameAction()
    {
        //Handle the enemy when it is out of frame
        if (!SpawnManager.IsWithinBoundaries(transform))
        {
            switch(outOfFrameAction)
            {
                case OutOfFrameAction.none: default:
                    break;
                case OutOfFrameAction.respawnAtEdge:
                    //If the enemy is outside the camera frame, teleport back to the edge of the frame.
                    transform.position = SpawnManager.GeneratePosition();
                    break;
                case OutOfFrameAction.despawn:
                    //Don't destroy it if it is spawned outside the frame.
                    if (!spawnedOutOfFrame)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }
        else
        {
            spawnedOutOfFrame = false;
        }
    }

    //creates knockback
    public virtual void Knockback(Vector2 velocity, float duration)
    {
        //no knockback if duration if > 0
        if (knockbackDuration > 0) 
        {
            return;
        }

        //knockback enemy
        knockbackVelocity = velocity;
        knockbackDuration = duration;   
    }

    public virtual void Move()
    {
        //Constantly move the enemy towards the player
        transform.position = Vector2.MoveTowards
            (transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);

        //flip sprite to face player
        if (transform.position.x < player.position.x)
        {
            sr.flipX = false; //Enemy is left of player. flips sprite to the right towards player
        }
        else
        {
            sr.flipX = true; //Enemy is right of player. flips sprite to the left towards player
        }
    }
}
