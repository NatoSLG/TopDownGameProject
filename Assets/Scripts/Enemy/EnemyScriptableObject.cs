using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]//creates a submenu for ScriptableObjects/Weapons
public class EnemyScriptableObject : ScriptableObject
{
    //scriptable objects save during runtime and will not be reset when the game is restarted

    //[SerializeField] allows the ability for the inspector window in unity to view a private variable
    [SerializeField] float speed;/*how fast the enemy will move*/
    [SerializeField] float maxHealth;
    [SerializeField] float damage;

    //setters and getters for each value
    public float Speed
    {
        get => speed;
        private set => speed = value;
    }

    public float MaxHealth
    {
        get => maxHealth;
        private set => maxHealth = value;
    }

    public float Damage
    {
        get => damage;
        private set => damage = value;
    }
}
