using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    //players stats
    float currentHealth;//health
    float currentRecovery;//health recovery
    float currentMoveSpeed;//move speed
    float currentMight;//overall damage delt
    float currentProjectileSpeed;//projectile speed
    float currentMagnet;//magnet radius

    #region Current Stats Properties
    public float CurrentHealth
    {
        get 
        { 
            return currentHealth; 
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (currentHealth != value) 
            {
                currentHealth = value;

                //print current stat
                if (GameManager.instance != null) 
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentRecovery
    {
        get
        {
            return currentRecovery;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (currentRecovery != value)
            {
                currentRecovery = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisply.text = "Recovery: " + currentRecovery;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            return currentMoveSpeed;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Movement Speed: " + currentMoveSpeed;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentMight
    {
        get
        {
            return currentMight;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (currentMight != value)
            {
                currentMight = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get
        {
            return currentProjectileSpeed;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
                //update the real time value of the stat and add any additions that need to be executed when value changes
            }
        }
    }

    public float CurrentMagnet
    {
        get
        {
            return currentMagnet;
        }

        //allows the logic to be changed whenever the stat has changed
        set
        {
            //Checks if the value has changed
            if (currentMagnet != value)
            {
                currentMagnet = value;

                //print current stat
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
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
    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        //assign stats to variables from the scriptable object
        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        //spawn starting weapon
        SpawnWeapon(characterData.StartingWeapon);
    }

    void Start()
    {
        //initialize the experience cap as the first experience cap increases
        experienceCap = levelRanges[0].experienceCapIncrease;

        //Display the current stats on start
        GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
        GameManager.instance.currentRecoveryDisply.text = "Recovery: " + currentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Movement Speed: " + currentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;

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

    void UpdateLevelText()
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
                Instantiate(damageEffect, transform.position, Quaternion.identity);
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
        healthBar.fillAmount = currentHealth / characterData.MaxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        //only heal the player if it is under max health
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;

            //sets the players health to max health if they heal above their max heal making sure they do not exceed max health
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }
    }
    //allows the players to heal over time based on the stat
    void Recover()
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;

            //sets the players health to max health if they heal above their max heal making sure they do not exceed max health
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }
    }

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
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); //adds weapon to the assigned inventory index/slot

        weaponIndex++; //ensures that each weapon is assigned to a different slot and prevents overlapping
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        //checks if the weapon slots are full
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("INVENTORY FULL");
            return;
        }

        //spawn the passive item
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);//sets as child of player
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); //adds passive item to the assigned inventory index/slot

        passiveItemIndex++; //ensures that each passive item is assigned to a different slot and prevents overlapping
    }
}
