using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemQuantity> Items = new List<ItemQuantity>();
    public List<Item> equipment = new List<Item>();

    public bool HasLight;

    public void UpdateStats(Combat combat)
    {
        // clear all combat effects
        combat.effects.RemoveAll(x => x.source == EffectSource.equipment);

        foreach (var item in equipment)
        {
            foreach (var changes in item.statChanges)
            {
                combat.effects.Add(new Effect("equip-" + changes.stat.ToString(), item.slot.ToString(), changes.stat, changes.change));
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

        equipment.Add(item);

        EventsDispatcher.inventoryUpdated();
    }
    
    public void UnEquipItem(EquipSlot slot, bool sendUpdate = true)
    {
        Item item = equipment.FirstOrDefault(i => i.slot == slot);

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
