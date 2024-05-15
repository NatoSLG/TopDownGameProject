using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to be atatched to all weapon prefabs. The weapon prefab works togeather w/ the
/// WeaponData ScriptableObjects to manage and run the status of all weapons. This is the keeps
/// track of the core functionallity of the weapon
/// </summary>
public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name;
        public string description;

        [Header("Visuals")]
        public Projectile projectilePrefab; //attached item will spawn a projectile every time the weapon cools down
        public Aura auraPrefab; //attached item will spawn an aura when weapon is equiped
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; //if 0, it will last forever
        public float damage;
        public float damageVariance;
        public float area;
        public float speed;
        public float cooldown;
        public float projectileInterval;
        public float knockback;
        public int number;
        public int piercing;
        public int maxInstances;

        //allows the ability to use + to add stats together when wanting to increase weapon stats
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result;
        }

        //get damage dealt
        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;
    public WeaponData data;
    protected float currentCooldown;
    protected PlayerMovement movement;

    //for dynamically created weapons, call initialise to set everything up
    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);

        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        currentCooldown = currentStats.cooldown;
    }

    protected virtual void Awake()
    {
        //Assign stats early as they will be used by other scripts later
        if (data)
        {
            currentStats = data.baseStats;
        }
    }

    protected virtual void Start()
    {
        //dont initialise the weapon if the weapon data is not assigned
        if (data)
        {
            Initialise(data);
        }
    }

    protected virtual void Update()
    {
        //once cooldown becomes 0, attack
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack(currentStats.number);
        }
    }

    //levels up weapon by 1 and calulates the corresponding stats
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        //prevents level up if already at the weapons max level
        if (!CanLevelUp()) 
        {
            Debug.Log(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        //add stats to the next level to the weapon
        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }

    //check if weapon can attack
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    /*preforms and attack with the weapon and returns true if the attack was successful.
    does not do anything and must be overridden at the child class to add a behaviour*/
    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }

    /*Gets the amount of damage that the weapon is supposed to deal.
     This factors in the weapon's stats, weapons variance, and characters Might stat.*/
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.CurrentMight;
    }

    //This retrieves weapon stats
    public virtual Stats GetStats()
    {
        return currentStats;
    }
}
