using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public EquipSlot slot;
    public List<StatChange> statChanges;
    public int maxHit;
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