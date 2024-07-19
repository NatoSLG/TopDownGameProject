using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for bother the Weapon and Passive Item Classes.
/// Primarily intended to handle weapon evolution as weapon and passive items will want to be evolvable
/// </summary>
public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1;
    public int maxLevel = 1;

    [HideInInspector]
    public ItemData data;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventoy inventory;
    protected PlayerStats owner;

    public PlayerStats Owner
    {
        get
        {
            return owner;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public string name;
        public string description;
    }

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;

        /*Store all evolution data.
         Data needs to be tracked to see if all catalysts are in the players inventory to allow evolution*/
        evolutionData = data.evolutionData;

        /*reference player and players inventory*/
        inventory = FindObjectOfType<PlayerInventoy>();
        owner = FindObjectOfType<PlayerStats>();
    }

    //call method to obtain all evolutions that the player can currently evolve to
    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        //check each item in list of evolutions and if it is the inventory
        foreach (ItemData.Evolution e in evolutionData) 
        {
            if (CanEvolve(e))
            {
                possibleEvolutions.Add(e);
            }
        }
        return possibleEvolutions.ToArray();
    }

    //checks if a specific evolution is possible
    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
    {
        //cannot evolve if the item hasnt reached the level to evolve
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning(string.Format("Evolution failed. Current level {0}, evolution level {1}", currentLevel, evolution.evolutionLevel));
            return false;
        }

        //check to see if all the catalysts are in inventory
        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level) 
            {
                Debug.LogWarning(string.Format("Evolution failed. Missing {0}", c.itemType.name));
                return false;
            }
        }

        return true;
    }

    //Spawn a new weapon for the character and remove all weapons that are supposed to be consumed
    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolutionData, levelUpAmount))
        {
            return false;
        }

        bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passives) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

        //Loop through all the cataysts and check if they need to be consumed
        foreach (ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if (c.itemType is PassiveData && consumePassives)
            {
                inventory.Remove(c.itemType, true);
            }
            if (c.itemType is WeaponData && consumeWeapons)
            {
                inventory.Remove(c.itemType, true);
            }
        }

        //Determins if the item should consume itself
        if (this is Passive && consumePassives)
        {
            inventory.Remove((this as Passive).data, true);
        }
        else if (this is Weapon && consumeWeapons)
        {
            inventory.Remove((this as Weapon).data, true);
        }

        //add new weapon to inventory
        inventory.Add(evolutionData.outcome.itemType);
        return true;
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    //whenever an item levels up, attempt to evolve it
    public virtual bool DoLevelUp()
    {
        if (evolutionData ==  null)
        {
            return true;
        }

        //attempts to evolve into every listed evolution of the weapon if the weapons evolution condition is leveling up
        foreach (ItemData.Evolution e in evolutionData)
        {
            if (e.condition == ItemData.Evolution.Condition.auto)
            {
                AttemptEvolution(e);
            }
        }
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
