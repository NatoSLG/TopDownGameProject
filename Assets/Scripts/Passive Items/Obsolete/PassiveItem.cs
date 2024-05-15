using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This is no longer in used due to the weapon revamp and updated scripts")]
public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    //function to apply the modifier withing the individual passive item scripts
    protected virtual void ApplyModifier()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        ApplyModifier();
    }
}
