using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneSwordController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedHurricane = Instantiate(weaponData.Prefab);//spawn the prefab as a gameobject
        spawnedHurricane.transform.position = transform.position; //assigns the position to be the same as the object parented to the player
        spawnedHurricane.transform.parent = transform; //assigns the prefab to spawn under
    }
}
