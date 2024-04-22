using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for bother the Weapon and Passive Item Classes.
/// Primarily intended to handle weapon evolution as weapon and passive items will want to be evolvable
/// </summary>
public class Item : MonoBehaviour
{
    public int currentLevel = 1;
    public int maxLevel = 1;

    protected PlayerStats owner;

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;
        owner = FindObjectOfType<PlayerStats>();
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    //whenever an item levels up, attempt to evolve it
    public virtual bool DoLevelUp()
    {
        //weapon evolution logic
        return true;
    }

    //effects received when equipping an item
    public virtual void OnEquip()
    {

    }

    //effects removed when unequipping an item
    public virtual void OnUnequip()
    {

    }
}
