using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount; //how many times the whip has been attacking in this iteration
    float currentSpawnYOffset; //if there are no more than 2 whips, start offsetting it upwards.

    protected override bool Attack(int attackCount = 1)
    {
        //if no projectile prefab is assigned, leave warning
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been assigned for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        //if there is no projectile assigned, set the weapon on cooldown
        if (!CanAttack())
        {
            return false;
        }

        /*if this is the first time the attack has been fired.
         Reset the currectSpawnCount.*/
        if (currentCooldown <= 0) 
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }

        /*calculate the angle and offset of the spawned projectile.
         if <currentSpawnCount> is even/ more than 1 more projectile,
        flip the direction of the spawn*/
        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2
            (spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax), currentSpawnYOffset);

        //spawn a copy of the projectile
        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)spawnOffset, Quaternion.identity);
        prefab.owner = owner; //set itself to be the owner

        //flip the projectile's sprite
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(-Mathf.Abs(prefab.transform.localScale.x), prefab.transform.localScale.y, prefab.transform.localScale.z);
        }

        //assign the stats
        prefab.weapon = this;
        ActivateCooldown(true);
        attackCount--;

        //determine where the next projetile should spawn
        currentSpawnCount++;
        if (currentCooldown > 1 && currentSpawnCount % 2 == 0) 
        {
            currentSpawnYOffset += 1;
        }

        //check if another attack needs to be done
        if (attackCount > 0) 
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }

        return true;
    }
}
