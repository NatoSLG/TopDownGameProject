using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinachPassiveItem : PassiveItem
{
    //increase the all damage delt by the player by the passive item scriptable object
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passiveItemData.Multiplier / 100f;
    }
}
