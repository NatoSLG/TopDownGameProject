using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This is being changed with the Weapon Class")]
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;//references stats
    float currentCD;//stores the current weapon cooldown

    protected PlayerMovement pm;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        currentCD = weaponData.WeaponCD;//makes sure that the weapon starts with the current cooldown
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //reduce the current cooldown until reaching 0 allowing attack
        currentCD -= Time.deltaTime;
        if (currentCD <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        currentCD = weaponData.WeaponCD;
    }
}
