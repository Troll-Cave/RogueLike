using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public EquipSlot slot;
    public List<StatChange> statChanges;
    public int maxHit;

    public bool stackable = false;
    public int quantity = 1;

    /// <summary>
    /// Basically this will be true if it's not a slot and it has stat changes attached to it
    /// </summary>
    public bool isConsumable
    {
        get { return slot == EquipSlot.none && (statChanges?.Count ?? 0) > 0; }
    }
}
// TODO: move this somewhere else
[Serializable]
public class ItemQuantity
{
    public int quantity;
    public Item Item;
}

[Serializable]
public struct StatChange
{
    public Stat stat;
    public short change;
}

public enum EquipSlot
{
    none,
    mainWeapon,
    offWeapon,
    helmet,
    chest,
    legs,
    gloves,
    boots,
    ammo,
    ring,
    neck,
}