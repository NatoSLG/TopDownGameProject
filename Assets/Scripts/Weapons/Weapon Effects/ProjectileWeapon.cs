using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval; //the time between attacks
    protected int currentAttackCount; //Number of times this attack will happen

    protected override void Update()
    {
        base.Update();

        //if the attack interval goes from > 0 to < 0, call Attack()
        if (currentAttackInterval > 0 ) 
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0 )
            {
                Attack(currentAttackCount);
            }
        }
    }

    /*only allows the weapon to fire when there is more than 1 attack being fired
     and the cooldown is <= 0 from base.CanAttack() from Weapon*/
    public override bool CanAttack()
    {
        if (currentAttackCount > 0)
        {
            return true;
        }

        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        //leave a warning message if there is no projectile prefab assigned
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        //check if the player can attack
        if (!CanAttack()) 
        {
            return false;
        }

        //calculate the angle and offset of the spawned projectile
        float spawnAngle = GetSpawnAngle();

        //spawn a copy of the projectile
        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle));

        //assigns the weapon prefab to the weapon and the owner of the prefab to owner
        prefab.weapon = this;
        prefab.owner = owner;

        ActivateCooldown(true);

        //reduce the number of attacks the we have to fire
        attackCount--;

        //check if the player can perform another attack
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }

        return true;
    }

    //get the direction the projectile should be facing when spawning
    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    /*Gerenates a random point to spawn the projectil on
     Rotates the facing of the point by spawnAngle*/
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
    }
}
