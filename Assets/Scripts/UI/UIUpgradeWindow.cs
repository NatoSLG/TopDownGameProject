using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Requires a VerticalLayoutGroup on the GameObject this is attached to.
/// It uses a component to make sure the buttons are evenly spaced out
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class UIUpgradeWindow : MonoBehaviour
{
    //access the passing/ spacing attributed on the layout
    VerticalLayoutGroup verticalLayout;

    //the button and tooltip template GameObjects that are assigned
    public RectTransform upgradeOptionTemplate;
    public TextMeshProUGUI tooltipTemplate;

    [Header("Settings")]
    public int maxOptions = 4; //limit the amount of options shown
    public string newText = "New!"; //the text that shows when a new upgrade is shown

    //color of the "New!" text and the regulare text
    public Color newTextColor = Color.yellow;
    public Color levelTextColor = Color.white;

    //the paths to the different UI elements in the <upgradeOptionTemplate>
    [Header("Paths")]
    public string iconPath = "Icon/Item Icon";
    public string namePath = "Name";
    public string descriptionPath = "Description";
    public string buttonPath = "Button";
    public string levelPath = "Level";

    /*Private variables that are used by the functions to track the status of diffent
    things in the UIUprgrageWindow*/
    RectTransform rectTransform; //the RectTransform for the element for easy reference
    float optionHight; //the default hight of the upgradeOptionTemplate
    int activeOptions; //tracks the number of options that are active currently

    //list of all the upgrade buttons on the window
    List<RectTransform> upgradeOptions = new List<RectTransform>();

    //tracks the screen width/height of the last frame.
    //detect the screen size change to know when to recalculate the size
    Vector2 lastScreen;

    /*Main function that will be called.
     Will need to specify which <inventory> to add the item to, and a list to all
    <possibleUpgrades> to show. Ite will select <pick> number of upgrades and show
    them. If a <tooltip> is specified, some text will appear at the bottom of the window*/
    public void SetUpgrades(PlayerInventoy inventory, List<ItemData> possibleUpgrades, int pick = 3, string tooltip = "")
    {
        pick = Mathf.Min(maxOptions, pick);

        //if there is not enought upgrade option boxes, create them
        if (maxOptions > upgradeOptions.Count) 
        {
            for (int i = upgradeOptions.Count; i < pick; i++)
            {
                GameObject go = Instantiate(upgradeOptionTemplate.gameObject, transform);
                upgradeOptions.Add((RectTransform)go.transform);
            }
        }
        print("Tooltip: " + tooltip);
        //if a string is provided, turn on the tooltip
        tooltipTemplate.text = tooltip;
        tooltipTemplate.gameObject.SetActive(tooltip.Trim() != "");

        /*activate only the number of upgrade options needed.
         arm the buttons and the different attributes like discriptions, ect.*/
        activeOptions = 0;
        int totalPossibleUpgrades = possibleUpgrades.Count; //how many possible upgrades to choose from

        foreach (RectTransform r in upgradeOptions)
        {
            if (activeOptions < pick && activeOptions < totalPossibleUpgrades)
            {
                r.gameObject.SetActive(true);

                //select one of the possible upgrades then remove it fromt he list
                ItemData selected = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                possibleUpgrades.Remove(selected);
                Item item = inventory.Get(selected);

                //Inset the name of the item
                TextMeshProUGUI name = r.Find(namePath).GetComponent<TextMeshProUGUI>();
                if (name)
                {
                    name.text = selected.name;
                }

                //Insert the current level of the item, or "New!" text if it is a new weapon
                TextMeshProUGUI level = r.Find(levelPath).GetComponent<TextMeshProUGUI>();
                if (level)
                {
                    if (item)
                    {
                        if (item.currentLevel >= item.maxLevel)
                        {
                            level.text = "Max!";
                            level.color = newTextColor;
                        }
                        else
                        {
                            level.text = selected.GetLevelData(item.currentLevel + 1).name;
                            level.color = levelTextColor;
                        }
                    }
                    else
                    {
                        level.text = newText;
                        level.color = newTextColor;
                    }
                }

                //Inset the description of the item
                TextMeshProUGUI desc = r.Find(descriptionPath).GetComponent<TextMeshProUGUI>();
                if (desc)
                {
                    if (item)
                    {
                        desc.text = selected.GetLevelData(item.currentLevel + 1).description;
                    }
                    else
                    {
                        desc.text = selected.GetLevelData(1).description;
                    }
                }

                //Insert the icon of the item
                Image icon = r.Find(iconPath).GetComponent<Image>();
                if (icon)
                {
                    icon.sprite = selected.icon;
                }

                //Insert the button action binding
                Button b = r.Find(buttonPath).GetComponent<Button>();
                if (b)
                {
                    b.onClick.RemoveAllListeners();
                    if (item)
                    {
                        b.onClick.AddListener(() => inventory.LevelUp(item));
                    }
                    else
                    {
                        b.onClick.AddListener(() => inventory.Add(selected));
                    }
                }

                activeOptions++;
            }
            else
            {
                r.gameObject.SetActive(false);
            }
        }

        //sizes all the elements so they do not exceed the size of the box
        RecalculateLayout();
    }

    /*Recalculates the hights of all elements.
     called whenyever the size of the window changes.
    this is done manually because the VerticallayoutGroup doesnt always
    space all the elements evenly*/
    void RecalculateLayout()
    {
        //calculates the total available height for all options then divides it by the number of options
        optionHight = (rectTransform.rect.height - verticalLayout.padding.top - verticalLayout.padding.bottom
            - (maxOptions - 1) * verticalLayout.spacing);

        if (activeOptions == maxOptions && tooltipTemplate.gameObject.activeSelf) 
        {
            optionHight /= maxOptions + 1;
        }
        else
        {
            optionHight /= maxOptions;
        }

        //Recalculates the hight of the tooltip as well if it is currently active
        if (tooltipTemplate.gameObject.activeSelf) 
        {
            RectTransform tooltipRect = (RectTransform)tooltipTemplate.transform;
            tooltipTemplate.gameObject.SetActive(true);
            tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, optionHight);
            tooltipTemplate.transform.SetAsLastSibling();
        }

        //sets the heigh of every active Upgrade Option button
        foreach (RectTransform r in upgradeOptions)
        {
            if (!r.gameObject.activeSelf)
            {
                continue;
            }
            r.sizeDelta = new Vector2(r.sizeDelta.x, optionHight);
        }
    }

    /*Checks if the last screen whidth/hight
     is the same as the current one.
    if not, the screen has changed sizes and will call RecalculateLayout()
    to update the hight of the buttons*/
    void Update()
    {
        //redraws the boxes in the element if the screen size changes
        if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            RecalculateLayout();
            lastScreen = new Vector2(Screen.width, Screen.height);
        }
    }

    void Awake()
    {
        //populates all important variables
        verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();

        if (tooltipTemplate)
        {
            tooltipTemplate.gameObject.SetActive(false);
        }

        if (upgradeOptionTemplate)
        {
            upgradeOptions.Add(upgradeOptionTemplate);
        }

        //Get the RectTransform of this object for the height calculations
        rectTransform = (RectTransform)transform;
    }

    /*convenience function to automatically populate vairables.
     it will autmatically search for a GameObject called "Upgrade Option"
    and assign it as the upgradeOptionTemplate, then search for a GameObject 
    "Tooltip" to be assigned as the tooltipTemplate*/
    void Reset()
    {
        upgradeOptionTemplate = (RectTransform)transform.Find("Upgrade Option");
        tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
    }
}
