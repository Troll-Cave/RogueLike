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
}

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
    helmet
}