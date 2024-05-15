using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This has been marked as obsolete and is being replaces with the CharacterData Script")]
[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/Character")]//creates a submenu for ScriptableObjects/Weapons
public class CharacterScriptableObject : ScriptableObject
{
    //scriptable objects save during runtime and will not be reset when the game is restarted

    //[SerializeField] allows the ability for the inspector window in unity to view a private variable
    [SerializeField] Sprite icon;
    [SerializeField] new string name;
    [SerializeField] GameObject startingWeapon;
    [SerializeField] float maxHealth;
    [SerializeField] float recovery;
    [SerializeField] float moveSpeed;
    [SerializeField] float might;
    [SerializeField] float projectileSpeed;
    [SerializeField] float magnet;

    //setters and getters for each value
    public Sprite Icon
    {
        get => icon;
        private set => icon = value;
    }

    public string Name
    {
        get => name;
        private set => name = value;
    }

    public GameObject StartingWeapon
    {
        get => startingWeapon;
        private set => startingWeapon = value;
    }

    public float MaxHealth
    {
        get => maxHealth;
        private set => maxHealth = value;
    }

    public float Recovery
    {
        get => recovery;
        private set => recovery = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        private set => moveSpeed = value;
    }

    public float Might
    {
        get => might;
        private set => might = value;
    }

    public float ProjectileSpeed
    {
        get => projectileSpeed;
        private set => projectileSpeed = value;
    }

    public float Magnet
    {
        get => magnet;
        private set => magnet = value;
    }
}
