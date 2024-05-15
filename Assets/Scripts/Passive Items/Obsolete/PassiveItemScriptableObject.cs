using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Will be replaced with PassiveData Class")]
[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]//creates a submenu for ScriptableObjects/Passive Item
public class PassiveItemScriptableObject : ScriptableObject
{
    //scriptable objects save during runtime and will not be reset when the game is restarted

    //[SerializeField] allows the ability for the inspector window in unity to view a private variable
    [SerializeField] float multiplier;
    [SerializeField] int level;//stores the level of the current weapon
    [SerializeField] GameObject nextLevelPrefab;//stores the upgraded version of the current weapon (not the prefab to be spawned)
    [SerializeField] new string name; //name of item
    [SerializeField] string description; //store the description of item and the description of the items upgrade
    [SerializeField] Sprite icon; //Used to change the image.sprite property. not ment to be modified in game, only in editor


    //setters and getters for each value
    public float Multiplier
    {
        get => multiplier;
        private set => multiplier = value;
    }
    public int Level
    {
        get => level;
        private set => level = value;
    }

    public GameObject NextLevelPrefab
    {
        get => nextLevelPrefab;
        private set => nextLevelPrefab = value;
    }
    public string Name
    {
        get => name;
        private set => name = value;
    }
    public string Description
    {
        get => description;
        private set => description = value;
    }

    public Sprite Icon
    {
        get => icon;
        private set => icon = value;
    }
}
