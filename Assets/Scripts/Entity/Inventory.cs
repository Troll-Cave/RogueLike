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

    public event System.Action inventoryUpdated;

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

        if (inventoryUpdated != null && sendUpdate)
        {
            inventoryUpdated();
        }
    }

    public void EquipItem(InventoryItem item)
    {
        UnEquipItem(item.slot);
    }
    
    public void UnEquipItem(EquipSlot slot)
    {

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

        if (inventoryUpdated != null && sendUpdate)
        {
            inventoryUpdated();
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
            return true;
        }

        return false;
    }
}
