using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GameObject that is spawned as an effect of a weapon firing. ex, projectiles, auras, pulses.
/// </summary>
public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector]
    public PlayerStats owner;
    [HideInInspector]
    public Weapon weapon;

    public PlayerStats Owner
    {
        get
        {
            return owner;
        }
    }

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}
