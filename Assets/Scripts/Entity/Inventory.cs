using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item HelmetSlot;
    public Item MainWeaponSlot;

    public List<ItemQuantity> Items = new List<ItemQuantity>();

    public bool HasLight;

    public void UpdateStats(Combat combat)
    {
        // clear all combat effects
        combat.effects.RemoveAll(x => x.source == EffectSource.equipment);

        if (HelmetSlot != null)
        {
            foreach (var changes in HelmetSlot.statChanges)
            {
                combat.effects.Add(new Effect("helmet-" + changes.stat.ToString(), "Helmet", changes.stat, changes.change));
            }
        }
    }

    public void RemoveItem(Item item, bool sendUpdate = true)
    {
        Items.RemoveAll(x => x.Item.name == item.name);

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void EquipItem(Item item)
    {
        RemoveItem(item, false);
        UnEquipItem(item.slot, false);

        // put on the thing
        if (item.slot == EquipSlot.helmet)
        {
            HelmetSlot = item;
        }
        else if (item.slot == EquipSlot.mainWeapon)
        {
            MainWeaponSlot = item;
        }

        EventsDispatcher.inventoryUpdated();
    }
    
    public void UnEquipItem(EquipSlot slot, bool sendUpdate = true)
    {
        Item item = null;

        if (slot == EquipSlot.helmet)
        {
            item = HelmetSlot;
            HelmetSlot = null;
        }
        else if (slot == EquipSlot.mainWeapon)
        {
            item = MainWeaponSlot;
            MainWeaponSlot = null;
        }

        Items.Add(new ItemQuantity
        {
            Item = item,
            quantity = item.quantity,
        });

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void AddItem(Item item, bool sendUpdate = true)
    {
        var currentItem = Items.FirstOrDefault(x => x.Item.name == item.name);

        if (currentItem != null)
        {
            currentItem.quantity += item.quantity;
        }
        else
        {
            Items.Add(new ItemQuantity
            {
                quantity = item.quantity,
                Item = item,
            });
        }

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="combat"></param>
    /// <returns>Whether or not to reload the UI</returns>
    
}
