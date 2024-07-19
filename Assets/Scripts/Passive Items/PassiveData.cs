using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// redesigned to replace the PassiveItemScriptableObject script. 
/// This will store all passive item level data in a single object insted of having multiple objects to store one item.
/// </summary>

[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-Down Rogue-Like/Passive Data")]
public class PassiveData : ItemData
{
    //stores base stats and levels
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1)
        {
            return baseStats;
        }

        //pick the stats from the next level
        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        //return an empty value and a warning
        Debug.LogWarning(string.Format("Passive does not have its level up stats configured for level {0}.", level));
        return new Passive.Modifier();
    }
}
