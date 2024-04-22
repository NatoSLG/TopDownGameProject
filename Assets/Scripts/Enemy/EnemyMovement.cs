using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform playerTransform;/*keeps track of players position*/

    Vector2 knockbackVelocity;
    float knockbackDuration;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        playerTransform = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if we are being knockbacked, process the knockback
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            /*moves the enemys position towards the players position constantly*/
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.transform.position, enemy.currentSpeed * Time.deltaTime);
        }
    }

    //creates knockback
    public void Knockback(Vector2 velocity, float duration)
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
}
