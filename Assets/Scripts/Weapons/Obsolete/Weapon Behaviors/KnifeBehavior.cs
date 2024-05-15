using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This is being changed with the Weapon Class")]
public class KnifeBehavior : ProjectileWeaponBehavior
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime; //sets the movement of the weapon. Multiplies the direction, set speed, and time
    }
}
