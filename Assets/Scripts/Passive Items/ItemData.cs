using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class for all weapons / passive items.
/// Base class can be used so that both WeaponData and PassiveItemData are used interchangeably in required
/// </summary>
public class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;
}
