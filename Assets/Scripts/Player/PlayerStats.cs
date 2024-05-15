using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

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

                //print current stat
                if (GameManager.instance != null) 
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format("Health: {0} / {1}", health, actualStats.maxHealth);
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float MaxHealth
    {
        get
        {
            return actualStats.maxHealth;
        }
        /*setting the current health will also update the UI interface on the pause screen.
        This allows the logic to be changed whenever the stat has changed*/
        set
        {
            //Checks if the value has changed
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format("Health: {0} / {1}", health, actualStats.maxHealth);
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }

    }
    public float CurrentRecovery
    {
        get
        {
            return Recovery;
        }
        set
        {
            Recovery = value;
        }
    }
     
    public float Recovery
    {
        get
        {
            return actualStats.recovery;
        }
        set
        {
            //Checks if the value has changed
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisply.text = "Recovery: " + actualStats.recovery;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            return MoveSpeed;
        }
        set
        {
            MoveSpeed = value;
        }
    }

    public float MoveSpeed
    {
        get
        {
            return actualStats.moveSpeed;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Movement Speed: " + actualStats.moveSpeed;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentMight
    {
        get
        {
            return Might;
        }
        set
        {
            Might = value;
        }
    }

    public float Might
    {
        get
        {
            return actualStats.might;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (actualStats.might != value)
            {
                actualStats.might = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + actualStats.might;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get
        {
            return Speed;
        }
        set
        {
            Speed = value;
        }
    }

    public float Speed
    {
        get
        {
            return actualStats.speed;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (actualStats.speed != value)
            {
                actualStats.speed = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentMagnet
    {
        get
        {
            return Magnet;
        }
        set
        {
            Magnet = value;
        }
    }

    public float Magnet
    {
        get
        {
            return actualStats.magnet;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }
    #endregion

    public ParticleSystem damageEffect;

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

        //assign stats to variables from the scriptable object
        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;
    }

    void Start()
    {
        //spawns starting weapon
        inventory.Add(characterData.StartingWeapon);

        //initialize the experience cap as the first experience cap increases
        experienceCap = levelRanges[0].experienceCapIncrease;

        //Display the current stats on start
        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.instance.currentRecoveryDisply.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Movement Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

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
            CurrentHealth -= damage;

            //creates particle effect if damage is taken
            if (damageEffect)
            {
                Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
            }

            invincibilityTimer = InvincibilityDuration;
            isInvincibile = true;

            if (CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
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
            GameManager.instance.AssignChosenWeaponAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
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
        }
    }
    //allows the players to heal over time based on the stat
    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            //sets the players health to max health if they heal above their max heal making sure they do not exceed max health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    [System.Obsolete("Old Function that is kept to maintain compatibility with the InventoryManager. Will be removed soon")]
    public void SpawnWeapon(GameObject weapon)
    {
        //checks if the weapon slots are full
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("INVENTORY FULL");
            return;
        }

        //spawn the starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);//sets as child of player
        //inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); //adds weapon to the assigned inventory index/slot

        weaponIndex++; //ensures that each weapon is assigned to a different slot and prevents overlapping
    }

    [System.Obsolete("No need to spawn passive items directly now")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        //checks if the weapon slots are full
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1) //must be -1 due to list starting from 0
        {
            Debug.LogError("INVENTORY FULL");
            return;
        }

        //spawn the passive item
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);//sets as child of player
        //inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); //adds passive item to the assigned inventory index/slot

        passiveItemIndex++; //ensures that each passive item is assigned to a different slot and prevents overlapping
    }
}
