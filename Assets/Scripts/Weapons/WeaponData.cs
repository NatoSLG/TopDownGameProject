using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//store all weapon evolution data into a single object rather than having multiple objects to store a single weapon

[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-Down Rogue-Like/Weapon Data")]
public class WeaponData : ItemData
{
    [HideInInspector]
    public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

    //provides stat growth and description of the next level
    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1)
        {
            return baseStats;
        }

        //pick the stats from the next level
        if (level - 2 < linearGrowth.Length) 
        {
            return linearGrowth[level - 2];
        }
        //pick one of the stats from random growth
        if (randomGrowth.Length > 0) 
        {
            return randomGrowth[Random.Range(0, randomGrowth.Length)];
        }
        //return if there is an empty value
        Debug.LogWarning(string.Format("Weapon does not have its level up stats configured for level {0}!", 0));
        return new Weapon.Stats();
    }
}
