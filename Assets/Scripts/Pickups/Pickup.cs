using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected PlayerStats target; //target the pickup will move towards
    protected float speed; //speed that the pickup will move at
    Vector2 initialPosition; //the initial position of the object
    float initialOffset;

    //Represent the bobbing animation of the object
    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }

    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public int experience;
    public int health;

    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if (target)
        {
            //move towards the player and check the distance between
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime) 
            {
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            //object is not moving towards the target and hangles animation
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin
                ((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;

            if (lifespan > 0)
            {
                this.lifespan = lifespan;
            }

            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    //detects when the pickup is destroyed and applies effects
    protected virtual void OnDestroy()
    {
        //checks if there is a target
        if (!target)
        {
            return;
        }
        //give player experience
        if (experience != 0) 
        {
            target.IncreaseExperience(experience);
        }
        //restores players health
        if (health != 0)
        {
            target.RestoreHealth(health);
        }
    }
}
