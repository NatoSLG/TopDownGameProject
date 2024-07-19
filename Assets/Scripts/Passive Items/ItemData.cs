using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class for all weapons / passive items.
/// Base class can be used so that both WeaponData and PassiveItemData are used interchangeably in required
/// </summary>
public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    [System.Serializable]
    public struct Evolution
    {
        public string name;
        
        public enum Condition
        {
            auto,
            treasureChest
        }
        public Condition condition;

        //each value in the enumeration represents a single bit, and multiple values can be combined using bitwise OR operations.
        [System.Flags] public enum Consumption
        {
            passives = 1,
            weapons = 2
        }
        public Consumption consumes;

        public int evolutionLevel;
        public Config[] catalysts;
        public Config outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }
    }

    public Evolution[] evolutionData;

    public abstract Item.LevelData GetLevelData(int level);
}
