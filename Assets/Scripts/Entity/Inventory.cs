using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryItem HelmetSlot;
    public InventoryItem MainWeaponSlot;

    public List<InventoryItem> Items = new List<InventoryItem>();
    public List<InventoryItem> currentDrops = new List<InventoryItem>();

    public bool HasLight;
    public int fullness = 10;

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

    public void RemoveItem(InventoryItem item, bool sendUpdate = true)
    {
        Items.RemoveAll(x => x.name == item.name);

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void EquipItem(InventoryItem item)
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
        InventoryItem item = null;

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

        Items.Add(item);

        if (sendUpdate)
        {
            EventsDispatcher.inventoryUpdated();
        }
    }

    public void AddItem(InventoryItem item, bool sendUpdate = true)
    {
        var currentItem = Items.FirstOrDefault(x => x.name == item.name);

        if (currentItem != null)
        {
            currentItem.quantity += item.quantity;
        }
        else
        {
            Items.Add(item);
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
    public bool Eat(Combat combat)
    {
        if (fullness > 0 && TurnManager.IsCalm())
        {
            var maxHealth = combat.GetStat(Stat.maxHealth);
            var health = combat.GetStat(Stat.health);

            if (health >= maxHealth)
            {
                return false;
            }

            var maxHealAmount = maxHealth - health;

            var healAmount = Mathf.Clamp(maxHealth / 10, 0, maxHealAmount);

            health += healAmount;

            TurnManager.AddMessage($"You heal for {healAmount}");

            combat.stats[Stat.health] = health;

            fullness--;

            DataManager.saveData.fullness = fullness;

            EventsDispatcher.statsChanged(combat);
            return true;
        }

        return false;
    }
}
