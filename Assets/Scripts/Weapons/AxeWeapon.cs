using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    //gives the spawn angle when instantiated
    protected override float GetSpawnAngle()
    {
        //calculates offset that is used to modify the angle of axes
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        //instantiates the first projectile at 90 degrees and modifies future axes during attack
        return 90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * offset); 
    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
    }
}
