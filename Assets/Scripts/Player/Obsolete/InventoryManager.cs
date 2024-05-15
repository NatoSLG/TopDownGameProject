using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete("This has been marked as obsolete and is being replaces with the PlayerInventory Script")]
public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<Image> passiveItemUISlots = new List<Image>(6);

    public int[] weaponLevels = new int[6];
    public int[] passiveItemLevels = new int[6];

    //will hold any data related to weapon upgrades
    [System.Serializable] //[System.Serializable] allows the ability to view and modify class instances in the inspector window
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;//Stores the index of the upgrade data
        public GameObject initialWeapon; //will hold the prefab for the inital weapon
        public WeaponScriptableObject weaponData; //will hold that data for the weapon
    }

    //will hold data related to passive item upgrades
    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;//Stores the index of the upgrade data
        public GameObject initialPassiveItem; //will hold the prefab for the inital item
        public PassiveItemScriptableObject passiveItemData; //will hold that data for the item
    }

    //hold the data related to each single upgrade option
    [System.Serializable]
    public class UpgradeUI
    {
        //used to display the upgrade name and description
        public TextMeshProUGUI upgradeNameDisplay;
        public TextMeshProUGUI upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    //list of upgrade options for weapon and passive items as well as a list of UI options that are present on screen
    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    public List<WeaponEvolutionBlueprint> weaponEvolutions = new List<WeaponEvolutionBlueprint>(); //holds all possible weapon evolutions that is created

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    //adds weapon to a specific index
    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true; //enable the image
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        //resumes gamplay if option is selected
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade) 
        {
            GameManager.instance.EndLevelUp();
        }
    }

    //adds passive item to a specific index
    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem) 
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true; //enable the image
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

        //resumes gamplay if option is selected
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];

            if (!weapon.weaponData.NextLevelPrefab)//checks if there is a next level for current weapon
            {
                Debug.LogError("NO NEXT LEVEL FOR " + weapon.name);
                return;
            }

            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity); //spawns in the game object of the upgraded weapon
            upgradedWeapon.transform.SetParent(transform); //set weapon to be child of player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());//add to players inventory in the same index/slot as the weapon being removed/upgraded
            Destroy(weapon.gameObject); //previous weapon is destroyed to prevent overlapping
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;//sets the weapon to the correct level

            //ensures that the chosen upgrade option recives the updated data
            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;

            //resumes gamplay if option is selected
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];

            if (!passiveItem.passiveItemData.NextLevelPrefab)//checks if there is a next level for current item
            {
                Debug.LogError("NO NEXT LEVEL FOR " + passiveItem.name);
                return;
            }

            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity); //spawns in the game object of the upgraded item
            upgradedPassiveItem.transform.SetParent(transform); //set item to be child of player
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());//add to players inventory in the same index/slot as the item being removed/upgraded
            Destroy(passiveItem.gameObject); //previous item is destroyed to prevent overlapping
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;//sets the item to the correct level

            //ensures that the chosen upgrade option recives the updated data
            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;

            //resumes gamplay if option is selected
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        //temporary lists to remove the duplicate upgrade options after that have been chosen
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            //check if there are no available weapon upgrades
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;

            //upgrade options are only shown as passive items if there are no weapon upgrades
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            //upgrade options are only shown as weapons if there are no passive item upgrades
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            //choose between weapon and passive item
            else 
            {
                upgradeType = Random.Range(1, 3);
            }

            //upgradeType returns a weapon if == 1
            if (upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)]; //assign to a random element from the weapon upgrade options list

                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);//remove the specific chosen upgrade from the list

                if (chosenWeaponUpgrade != null) 
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newWeapon = false; //determins if the weapon is new or existing
                    
                    //searches for a weapon slot that matches the weapon upgrade to determin if it new or existing
                    for (int i = 0; i < weaponSlots.Count; i++) 
                    {
                        //checks if the slots weapon data is not null & matches the data of the chosen weapon
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;

                            if (!newWeapon)
                            {
                                if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)//prevention from leveling up weapon if there are no more levels for the weapon
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex)); //Calling LevelUpWeapon() on click

                                //sets the description and name display to their next level's descrition and name display
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description; //update description
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name; //update name
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }

                    //spawn a new weapon and sets the initial description and name
                    if (newWeapon == true)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon)); //calling SpawnWeapon() on click
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description; //set description
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name; //set name
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon; //sets the sprite
                }
            }
            //upgradeType returns a Passive Item if == 2
            else if (upgradeType == 2) 
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)]; //assign to a random element from the item upgrade options list

                availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade);//removes specific chosen upgrade from list

                if (chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newPassiveItem = false; //determins if the item is new or existing

                    //searches for a item slot that matches the item upgrade to determin if it new or existing
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        //checks if the slots item data is not null & matches the data of the chosen item
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;

                            if (!newPassiveItem)
                            {
                                if (!chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)//prevention from leveling up weapon if there are no more levels for the weapon
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex)); //Calling LevelUpPassiveItem() on click

                                //sets the description and name display to their next level's descrition and name display
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description; //update description
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name; //update name
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }

                    //spawn a new passive item and sets the initial description and name
                    if (newPassiveItem == true)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem)); //calling SpawnPassiveItem() on click
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description; //set description
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name; //set name
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon; //sets the sprite
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);//call DisbaleUpgradeUI() to disable all ui before applying upgrades to them
        }
    }

    //single meathod to remove and apply upgrades using SendMessage()
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

    public List<WeaponEvolutionBlueprint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBlueprint> possibleEvolutions = new List<WeaponEvolutionBlueprint>(); //stores all weaponEvolutionBlueprints that matches the state of the players inventory

        //check weapon controller to see if it matches any of the evolution blueprints
        foreach (WeaponController weapon in weaponSlots)
        {
            if(weapon != null)
            {
                //check each passive item to see if it matches any of the evolution blueprints
                foreach (PassiveItem catalyst in passiveItemSlots)
                {
                    if(catalyst != null)
                    {
                        //search through each WeaponEvolutionBlueprint for each combo of weapon and passive item to check if it matches the blueprint's baseWeaponData and catalyst
                        foreach (WeaponEvolutionBlueprint evolution in weaponEvolutions)
                        {
                            if (weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passiveItemData.Level >= evolution.catalystPassiveItemData.Level)
                            {
                                //if weapon evoltion match is found, add to possible evoltion list
                                possibleEvolutions.Add(evolution);
                            }
                        }
                    }
                }
            }
        }
        return possibleEvolutions;
    }

    public void EvolveWeapon(WeaponEvolutionBlueprint evolution)
    {
        //loop through each weapon slot and check for potential evolutions
        for (int weaponSlotIndex = 0; weaponSlotIndex < weaponSlots.Count; weaponSlotIndex++) 
        {
            WeaponController weapon = weaponSlots[weaponSlotIndex];

            if (!weapon)
            {
                continue;
            }

            //loop through each catalyst slot and check for potential evolutions
            for (int catalystSlotIndex = 0; catalystSlotIndex < passiveItemSlots.Count; catalystSlotIndex++)
            {
                PassiveItem catalyst = passiveItemSlots[catalystSlotIndex];

                if (!catalyst)
                {
                   continue;
                }

                //if weapon and catalyst are both able to evolve
                if (weapon && catalyst && weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passiveItemData.Level >= evolution.catalystPassiveItemData.Level)
                {
                    GameObject evolveWeapon = Instantiate(evolution.evolvedWeapon, transform.position, Quaternion.identity);
                    WeaponController evolvedWeaponController = evolveWeapon.GetComponent<WeaponController>();

                    evolveWeapon.transform.SetParent(transform); //sets object to be the child of the player
                    AddWeapon(weaponSlotIndex, evolvedWeaponController);//replaces the base weapon with the evolved weapon
                    Destroy(weapon.gameObject);//destroys base weapon

                    /*Levels and UI icon changes*/
                    weaponLevels[weaponSlotIndex] = evolvedWeaponController.weaponData.Level;
                    weaponUISlots[weaponSlotIndex].sprite = evolvedWeaponController.weaponData.Icon;

                    //update upgrade options and remove upgrade from list
                    weaponUpgradeOptions.RemoveAt(evolvedWeaponController.weaponData.EvolveUpgradeToRemove);

                    Debug.LogWarning("WEAPON EVOLVED");

                    return;
                }
            }
        }
    }
}
