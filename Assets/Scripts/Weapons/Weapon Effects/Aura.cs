using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Aura is a DOT effect that applies to a specifiec area in timed intervals.
/// </summary>
public class Aura : WeaponEffect
{
    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>(); //list of enemies to remove from the dictionary

    // Update is called once per frame
    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        /*Loop through every target affected by the aura.
          Reduce the cooldown of the aura for it.
          If the cooldown reaches 0, deal damage to it.*/
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargets.ToList())
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    //if the target is marked for removal, remove it
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    //reset the cooldown and deal damage
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown * Owner.Stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.TryGetComponent(out EnemyStats es))
        {
            //if the target is not yet affected by this aura, add it to the list of affected targets
            if (!affectedTargets.ContainsKey(es))
            {
                //Always starts with an interval of 0 so that it will get damaged in the next Update()
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(es))
                {
                    targetsToUnaffect.Remove(es);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.TryGetComponent(out EnemyStats es))
        {
            //do not directly remove the target upon leaving because the enemy's cooldowns still have to be tracked
            if (affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}
