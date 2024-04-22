using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(weaponData.Prefab);
        spawnedKnife.transform.position = transform.position; //assigns the position to be the same as the object parented to the player
        spawnedKnife.GetComponent<KnifeBehavior>().DirectionChecker(pm.lastMovedVector);//reference and set direction
    }
}
