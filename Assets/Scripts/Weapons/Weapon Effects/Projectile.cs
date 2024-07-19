using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to all projectile prefabs.
/// All-purpose script to modify the status of all projectile weapons 
/// All spawned projectiles will shoot in the direction the player is facing and deal damage to the object hit
/// </summary>
public class Projectile : WeaponEffect
{
    /*Enumerator to decaler projectile and owner.
     use this to determine when a knockback comes from the player or the projectile weapon*/
    public enum DamageSource
    {
        projectile,
        owner,
    }

    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3 (0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing; //retrieved from the stats of the weapon, this calculates how many times the weapon can go through something

    //Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed * weapon.Owner.Stats.speed;
        }

        //prevent the area from being 0, as it highes the projectile
        float area = weapon.GetArea();

        if (area <= 0)
        {
            area = 1;
        }

        transform.localScale = new Vector3(area * Mathf.Sign(transform.localScale.x), area * Mathf.Sign(transform.localScale.y), 1);

        //set how much piercing this object has
        piercing = stats.piercing;

        //destroy the projectile after its lifespan expires
        if (stats.lifespan > 0)
        {
            Destroy(gameObject, stats.lifespan);
        }

        //if projectil is auto-aiming, automatically find a suitable enemy
        if (hasAutoAim)
        {
            AcquireAutoAimFacing();
        }
    }

    //if the projectile is homing, meathod to find a suitable target to move towards
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; //determins where to aim

        //find all enemies on screen
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        //select a random enemy, if at least 1
        //if there is no enemy, pick a random angle
        if (targets.Length > 0) 
        {
            EnemyStats selectedTaget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTaget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        //point the projectile towards where we are aiming at
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    //Update is called once per frame
    protected virtual void FixedUpdate()
    {
        //only drive movement ourselves if this is a kinematic. Unity does not control movement of kinematic and needs to be done by script
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * weapon.Owner.Stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        EnemyStats es = c.GetComponent<EnemyStats>();
        BreakableProps p = c.GetComponent<BreakableProps>();

        //only collide with enemies or breakable props
        if (es)
        {
            /*if there is an owner, and the damage source is set to owner,
             calculate the knockback using the owner instead of projectile*/
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            //deals damage and destorys the projectile
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            piercing--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }    
        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

            //Destroy this object if it has run out of health from hitting other stuff
            if (piercing <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
