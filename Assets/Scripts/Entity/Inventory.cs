using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemQuantity> items = new List<ItemQuantity>();
    public List<Item> equipment = new List<Item>();

    public bool HasLight;

    private Combat playerCombat;

    private void Awake()
    {
        playerCombat = GetComponent<Combat>();
    }

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

    public void RemoveItem(Item item, int count = 1, bool sendUpdate = true)
    {
        var tempItem = GetItem(item);

        tempItem.quantity -= count;
        
        if (tempItem.quantity <= 0)
        {
            items.Remove(tempItem);
        }

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void EquipItem(Item item)
    {
        RemoveItem(item, 1, false);
        UnEquipItem(item.slot, false);

        equipment.Add(item);

        EventsDispatcher.inventoryUpdated();
    }
    
    public void UnEquipItem(EquipSlot slot, bool sendUpdate = true)
    {
        Item item = equipment.FirstOrDefault(i => i.slot == slot);

        if (item == null)
        {
            return;
        }

        AddItem(item, false);
        equipment.RemoveAll(x => x.slot == slot);

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void AddItem(Item item, bool sendUpdate = true)
    {
        var currentItem = items.FirstOrDefault(x => x.item.name == item.name);

        if (currentItem != null)
        {
            currentItem.quantity += item.quantity;
        }
        else
        {
            items.Add(new ItemQuantity
            {
                quantity = item.quantity,
                item = item,
            });
        }

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void UseItem(Item item)
    {
        var itemQuantity = GetItem(item.GetInstanceID());

        if (itemQuantity != null)
        {
            foreach (var statChange in itemQuantity.item.statChanges)
            {
                if (statChange.stat == Stat.fullness)
                {
                    playerCombat.stats[Stat.fullness] += statChange.change;
                }
            }

            itemQuantity.quantity -= 1;

            if (itemQuantity.quantity <= 0)
            {
                items.Remove(itemQuantity);
            }

            EventsDispatcher.inventoryUpdated();
            EventsDispatcher.statsChanged(playerCombat);
        }
    }

    private ItemQuantity GetItem(int id)
    {
        return items.FirstOrDefault(x => x.item.GetInstanceID() == id);
    }
    private ItemQuantity GetItem(Item item)
    {
        return GetItem(item.GetInstanceID());
    }
}
