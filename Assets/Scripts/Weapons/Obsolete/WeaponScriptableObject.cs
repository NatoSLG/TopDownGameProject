using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the WeaponData class.")]
[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapons")]//creates a submenu for ScriptableObjects/Weapons
public class WeaponScriptableObject : ScriptableObject 
{
    //scriptable objects save during runtime and will not be reset when the game is restarted

    //[SerializeField] allows the ability for the inspector window in unity to view a private variable
    [SerializeField] GameObject prefab;//stores the weapon prefab
    //base stats
    [SerializeField] float damage;//stores base damage
    [SerializeField] float speed;//stores base speed
    [SerializeField] float weaponCD;//stores weapon cooldown
    [SerializeField] int pierce;//stores the max amount of time a weapon can hit an enemy before it gets destoryed
    [SerializeField] int level;//stores the level of the current weapon
    [SerializeField] GameObject nextLevelPrefab;//stores the upgraded version of the current weapon (not the prefab to be spawned)
    [SerializeField] new string name; //name of weapon
    [SerializeField] string description; //store the description of weapon and the description of the weapons upgrade
    [SerializeField] Sprite icon; //Used to change the image.sprite property. not ment to be modified in game, only in editor
    [SerializeField] int evolveUpgradeToRemove;//indicate the weapon's index which upgrade to be removed after evolution

    //setters and getters for each value
    public GameObject Prefab
    {
        get => prefab;
        private set => prefab = value;
    }

    public float Damage
    {
        get => damage;
        private set => damage = value;
    }

    public float Speed
    {
        get => speed;
        private set => speed = value;
    }

    public float WeaponCD
    {
        get => weaponCD;
        private set => weaponCD = value;
    }

    public int Pierce
    {
        get => pierce;
        private set => pierce = value;
    }

    public int Level
    {
        get => level;
        private set => level = value;
    }

    public GameObject NextLevelPrefab
    {
        get => nextLevelPrefab;
        private set => nextLevelPrefab = value;
    }
    public string Name
    {
        get => name;
        private set => name = value;
    }
    public string Description
    {
        get => description;
        private set => description = value;
    }

    public Sprite Icon
    {
        get => icon;
        private set => icon = value;
    }

    public int EvolveUpgradeToRemove
    {
        get => evolveUpgradeToRemove;
        private set => evolveUpgradeToRemove = value;
    }
}
