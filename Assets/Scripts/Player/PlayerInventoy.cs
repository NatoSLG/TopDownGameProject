using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

        public void Assign(Item assignedItem)
        {
            item = assignedItem;

            //assigns the weapon or passive item to the inventory and retrieves the data for the icon to set the image
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
            }
            else
            {
                Passive p = item as Passive;
            }

            Debug.Log(string.Format("Assigned {0} to player", item.name));
        }

        public void Clear()
        {
            item = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);
    public UIInventoryIconsDisplay weaponUI;
    public UIInventoryIconsDisplay passiveUI;

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>(); //List of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); // list of upgrade options for passive items

    public UIUpgradeWindow upgradeWindow;

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
            if (p && p.data == type)
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
            if (w && w.data == type)
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
            spawnedWeapon.transform.SetParent(transform); //set the wapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            //assign the weapon to the slot
            weaponSlots[slotNum].Assign(spawnedWeapon);
            weaponUI.Refresh();

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
        passiveUI.Refresh();

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

    /*Overload so the both ItemData or Item can be used to
     level up and item in the inventory*/
    public bool LevelUp(ItemData data) 
    {
        Item item = Get(data);
        if (item)
        {
            return LevelUp(item);
        }
        return false;
    }

    //Levels up a selected weapon in the players inventory
    public bool LevelUp(Item item) 
    {
        //tries to level up the item
        if (!item.DoLevelUp())
        {
            Debug.LogWarning(string.Format("Failed to level up {0}.", item.name));
            return false;
        }

        weaponUI.Refresh();
        passiveUI.Refresh();

        //close the level up screen afterwards
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        //if it is a passive item, recalculate players stats
        if (item is Passive)
        {
            player.RecalculateStats();
        }
        return true;
    }

    //checks a list of slots to see if there are any slots left
    int GetSlotsLeft(List<Slot> slots)
    {
        int count = 0;

        foreach (Slot s in slots) 
        {
            if (s.IsEmpty())
            {
                count++;
            }
        }
        return count;
    }

    //Determines what upgrade options should appear
    void ApplyUpgradeOptions()
    {
        /*<availableUpgrades> is an emply list that will be filtered from
         <allUpgrades>, which is the list of all upgrades in PlayerInventory.
        Not all upgrades can be applied, as some may have already been maxed out
        or the player may not have enough inventory slots*/
        List<ItemData> availableUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        //how many weapon and passive slots are left
        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        /*Filters through the available weapons and passives.
         Adds those that can possible be an option*/
        foreach (ItemData data in allUpgrades) 
        {
            /*if a weapon of this type exists, allow for the upgrade if the
             level of the weapon is not already maxed out*/
            Item obj = Get(data);
            if (obj)
            {
                if (obj.currentLevel < data.maxLevel)
                {
                    availableUpgrades.Add(data);
                }
            }
            else
            {
                /*if the item is still not in the inventory, check if there
                 still is enough slots to take the new item*/
                if (data is WeaponData && weaponSlotsLeft > 0)
                {
                    availableUpgrades.Add(data);
                }
                else if (data is PassiveData && passiveSlotsLeft > 0)
                {
                    availableUpgrades.Add(data);
                }
            }
        }

        //show the UI upgrade window if there are still available upgrades left
        int availUpgradeCount = availableUpgrades.Count;
        if (availUpgradeCount > 0) 
        {
            print("Avail upgrades: " + string.Join(", ", availableUpgrades.Select(upgrade => upgrade.ToString()).ToArray()));
            bool getExtraItem = 1f - 1f / player.Stats.luck > UnityEngine.Random.value;
            if (getExtraItem)
            {
                upgradeWindow.SetUpgrades(this, availableUpgrades, 4);
            }
            else 
            {
                upgradeWindow.SetUpgrades(this, availableUpgrades, 3,
                    "Increase you Luck stat for a chance to get 4 items!");
            }
        }
        else if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        ApplyUpgradeOptions();
    }
}
