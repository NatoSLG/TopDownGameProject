using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that takes a PassiveData and is used to increase the player stats when recieved
/// </summary>
public class Passive : Item
{
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]
    public class Modifier : LevelData
    {
        public CharacterData.Stats boosts;
    }

    //for dynamically created passives, call initialise to set everything up
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    //levels up the weapon by 1 and calculates the corresponding stats
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        //prevent level up if we are already at max level
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        //add stats of the next level to weapon
        currentBoosts += ((Modifier)data.GetLevelData(++currentLevel)).boosts;
        return true;
    }
}
