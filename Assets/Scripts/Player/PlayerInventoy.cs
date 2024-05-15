using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// redesigned to replace InventoryManager script.
/// The goal is to reduce the amount of variable and to allow more readability in the inspector
/// </summary>
public class PlayerInventoy : MonoBehaviour
{
    [System.Serializable]
    /*Nested class that is used to represent an inventory slot.
     Inventory can contain an item or weapon and needs to track the UI equivalent(image) of the slot*/
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;

            //assigns the weapon or passive item to the inventory and retrieves the data for the icon to set the image
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }

            Debug.Log(string.Format("Assigned {0} to player", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>(); //List of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); // list of upgrade options for passive items
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); //List of ui for upgrade options present in the scene

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    //checks if the inventory has an item of a certain type
    public bool Has(ItemData type)
    {
        return Get(type);
    }

    public Item Get(ItemData type) 
    {
        if (type is WeaponData)
        {
            return Get(type as WeaponData);
        }
        else if(type is PassiveData) 
        {
            return Get(type as PassiveData);
        }
        return null;
    }

    //find a passive of a certain type in the inventory
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
            {
                return p;
            }
        }
        return null;
    }
    
    // Find a weapon of a certain type in the inventory
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    //removes a weapon of a certain type, specified by <data>
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        //remove this weapon from the upgrade pool
        if  (removeUpgradeAvailability)
        {
            availableWeapons.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++) 
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }

    //removes a passive of a certain type, specified by <data>
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        //remove this weapon from the upgrade pool
        if (removeUpgradeAvailability)
        {
            availablePassives.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Passive p = weaponSlots[i].item as Passive;
            if (p.data == data)
            {
                weaponSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;
    }

    /*if an ItemData is passed, determine what type it is and call the respective overload.
     have an option bool to remove this tiem from the upgrade list*/
    public bool Remove(ItemData data, bool removeUpgradeAvailability = false) 
    {
        if (data is PassiveData)
        {
            return Remove(data as PassiveData, removeUpgradeAvailability);
        }
        else if (data is WeaponData)
        {
            return Remove(data as WeaponData, removeUpgradeAvailability);
        }
        return false;
    }

    /*Find an empty slot and adds a weapon of a certain typ
     returns the slot number that the item was put in*/
    public int Add(WeaponData data) 
    {
        int slotNum = -1;

         //try to find an empty slot
         for (int i = 0; i < weaponSlots.Capacity; i++) 
         {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
         }

         //if there is no emply slot, exit
         if (slotNum < 0)
         {
            return slotNum;
         }

        /*create the weapon in the slot
         get the type of the weapon we want to spawn*/
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            //spawn the weapon GameObject
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform); //set the wapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            //assign the weapon to the slot
            weaponSlots[slotNum].Assign(spawnedWeapon);

            //close the level up UI if it is open
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type spcified for {0}.", data.name));
        }
        return -1;
    }

    /* Finds an empty slot and adds a passive of a certain type
     Returns the slot number that the item was put in*/
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        //try to find an empty slot
        for (int i = 0; i < passiveSlots.Capacity; i++) 
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        //if there is no emply slot, exit
        if (slotNum < 0)
        {
            return slotNum;
        }

        /*Create the passive in the slot.
         Get the type of the passive that is wanted to be spawned*/
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); //set the weapon to be a child of the player
        p.transform.localPosition = Vector2.zero;

        //assign the passive to the slot
        passiveSlots[slotNum].Assign(p);

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }

    //if what is being added is unknown, this will determine that
    public int Add(ItemData data) 
    {
        if (data is WeaponData)
        {
            return Add(data as WeaponData);
        }
        else if (data is PassiveData)
        {
            return Add(data as PassiveData);
        }
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            //dont level up the weapon if it is already at the max level
            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", weapon.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex) 
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;

            //dont level up if it is already at the max level
            if (!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", p.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    //determine what upgrade options should appear
    void ApplyUpgradeOptions()
    {
        //Make a duplicate of the available weapon/passive upgrade list to be iterated through in the function
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveItemUpgrades = new List<PassiveData>(availablePassives);

        //iterate through each slot in the upgrade ui
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            //if there are no more available upgrades, stop
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

            //determine whether this upgrade should be for passive or active weapon
            int upgradeType;
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if( availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                //randomly ganerates a number between 1 - 2
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            //Generates an active weapon upgrade
            if (upgradeType == 1)
            {
                //pick a weapon upgrade, the remove it so that it does not appear twice
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                //ensure that the selected weapon sata is valid
                if (chosenWeaponUpgrade != null) 
                {
                    //turn on the UI slot
                    EnableUpgradeUI(upgradeOption);

                    /*loops through all the existing weapons.
                     if a match is found, hook an event listener to the button that will level up the weapon when the button is clicked*/
                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++) 
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade) 
                        {
                            //if the weapon is already at the max level, do not allow upgrade
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            //set the event listener, item, and level description to be of the next level
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i)); //applies button functionality
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    //if reached, add a new weapon instead of upgreading an existing weapon
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade)); //Applies button functionality
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description; //applies initial description
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name; //applies initial name
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if(upgradeType == 2)
            {
                /*NOTE: must be recoded in the future.
                 currently, it disables an upgrade slot if it hits a weapon that has already reached max level*/
                PassiveData chosenPassiveUpgrade = availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null) 
                {
                    //turn on UI slot
                    EnableUpgradeUI(upgradeOption);

                    /*loops through all the existing passive items.
                     if a match is found, hook an event listener to the button that will level up the weapon when the button is clicked*/
                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++) 
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            //if the passive is already at the max level, do not allow upgrade
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i)); //applies button functionality
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade)); //applies button functionality
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description; //applies initial description
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name; //applies initial name
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); //calls the DisableUpgradeUI function to disable all UI options before applying upgrades to them
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
