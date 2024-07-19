using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Damage does not scale with might state currently
public class LightningRingWeapon : ProjectileWeapon
{
    List<EnemyStats> allSelectedEnemies = new List<EnemyStats>();

    protected override bool Attack(int attackCount = 1)
    {
        //if there is no projectile prefab assigned, leave a warning message
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning(string.Format("Hit effect prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        //if there is no projectile assigned, set the weapon on cooldown
        if (!CanAttack())
        {
            return false;
        }

        /*if the cooldown is < 0 & this is the first time the attack has been fired,
         refresh the array of enemies selected.*/
        if (currentCooldown <= 0)
        {
            allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());
            ActivateCooldown(true);
            currentAttackCount = attackCount;
        }

        //Find an enemy on screen to strike with lightning
        EnemyStats target = PickEnemy();
        if (target) 
        {
            DamageArea(target.transform.position, GetArea(), GetDamage());
            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }

        //If we have more than one attack count
        if (attackCount > 0) 
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }
        return true;
    }

    //randomly picks an enemy on screen
    EnemyStats PickEnemy()
    {
        EnemyStats target = null;
        while (!target && allSelectedEnemies.Count > 0) 
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            //if the target is already dead, remove and skip it
            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue;
            }

            /*Check if the enemy is on screen.
             If the enemy is missing a renderer, it cannot be struck, as we cannot check
            whether it is on screen or not*/
            Renderer r = target.GetComponent<Renderer>();
            if (!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                target = null;
                continue;
            } 
        }

        allSelectedEnemies.Remove(target);
        return target;
    }

    //deal damage in an striked area
    void DamageArea(Vector2 position, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);

        foreach (Collider2D t in targets) 
        {
            EnemyStats es = t.GetComponent<EnemyStats>();
            if (es)
            {
                es.TakeDamage(damage,transform.position);
            }
        }
    }
}
