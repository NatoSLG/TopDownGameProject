using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingsPassiveItem : PassiveItem
{
    //increase the move speed provided by the passive item scriptable object
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f;
    }
}
