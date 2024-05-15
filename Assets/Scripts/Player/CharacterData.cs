using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// redesigned to replace CharacterScriptableObject script.
/// This inputs all the stats of the character inside a struct that can be also used for passives.
/// This also changes the starting weapons variable from a GameObject to WeaponData
/// </summary>
[CreateAssetMenu(fileName = "Character Data", menuName = "2D Top-Down Rogue-Like/Character Data")]
public class CharacterData : ScriptableObject
{
    //[SerializeField] allows the ability for the inspector window in unity to view a private variable
    [SerializeField] Sprite icon;
    [SerializeField] new string name;
    [SerializeField] WeaponData startingWeapon;

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

    public WeaponData StartingWeapon
    {
        get => startingWeapon;
        private set => startingWeapon = value;
    }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth;
        public float recovery;
        public float moveSpeed;
        public float might;
        public float speed;
        public float magnet;

        //sets the default value for the stats
        public Stats(float maxHealth = 1000, float recovery = 0, float moveSpeed = 5f, float might = 1f, float speed = 1f, float magnet = 1.6f)
        {
            this.maxHealth = maxHealth;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.might = might;
            this.speed = speed;
            this.magnet = magnet;
        }

        //allows the ability to use + to add stats together when wanting to increase character stats stats
        public static Stats operator +(Stats s1, Stats s2) 
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            return s1;
        }
    }
    public Stats stats = new Stats(1000);
}
