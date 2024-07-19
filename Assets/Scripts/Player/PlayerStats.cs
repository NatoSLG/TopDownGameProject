using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get
        {
            return actualStats;
        }
        set 
        {
            actualStats = value;
        }
    }

    float health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get 
        { 
            return health; 
        }
        /*setting the current health will also update the UI interface on the pause screen.
        This allows the logic to be changed whenever the stat has changed*/
        set
        {
            //Checks if the value has changed
            if (health != value) 
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }

    #endregion

    [Header("Visuals")]
    public ParticleSystem damageEffect; //If damage dealt to player
    public ParticleSystem blockedEffect; //If armor blocks damage 

    //experience and level of the player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    //class that defines a level range and experience cap for the level range and serializes it to access it
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    //IFrames
    [Header("I-Frames")]
    public float InvincibilityDuration;
    float invincibilityTimer;
    bool isInvincibile;

    public List<LevelRange> levelRanges;

    //inventory
    PlayerCollector collector;
    PlayerInventoy inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    void Awake()
    {
        characterData = CharacterSelector.GetData();

        //checks if instance exist
        if (CharacterSelector.instance)
        {
            CharacterSelector.instance.DestroySingleton();
        }
        
        inventory = GetComponent<PlayerInventoy>();
        collector = GetComponentInChildren<PlayerCollector>();

        //assign stats to variables from the scriptable object
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    void Start()
    {
        //spawns starting weapon
        inventory.Add(characterData.StartingWeapon);

        //initialize the experience cap as the first experience cap increases
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        //UI
        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    private void Update()
    {
        if (invincibilityTimer > 0) 
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincibile) //allows the player to recieve one instance of damage at a time
        {
            isInvincibile = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventoy.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        collector.SetRadius(actualStats.magnet);
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
        UpdateExpBar();
    }

    //checks if the player is >= the experience cap
    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            GameManager.instance.StartLevelUp();

            if (experience >= experienceCap)
            {
                LevelUpChecker();
            }
        }
    }

    void UpdateExpBar()
    {
        //update exp bar fill amount
        expBar.fillAmount = (float)experience / experienceCap;
    }

    public void UpdateLevelText()
    {
        levelText.text = "LVL " + level.ToString();
    }

    public void TakeDamage(float damage)
    {
        //check if player is not invincible. if not, reduce health and grant invincibility
        if (!isInvincibile)
        {
            //Take armor into account before dealing deal damage to player
            damage -= actualStats.armor;

            if (damage > 0)
            {
                CurrentHealth -= damage;

                //creates assigned particle effect if damage is taken
                if (damageEffect)
                {
                    Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
                }

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                //Play assigned particle effect
                if (blockedEffect)
                {
                    Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
                }
            }

            invincibilityTimer = InvincibilityDuration;
            isInvincibile = true;
        }
    }

    void UpdateHealthBar()
    {
        //update the fil amount of the health bar
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        //only heal the player if it is under max health
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            //sets the players health to max health if they heal above their max heal making sure they do not exceed max health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }
    //allows the players to heal over time based on the stat
    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;

            //sets the players health to max health if they heal above their max heal making sure they do not exceed max health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }
}
