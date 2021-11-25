using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item HelmetSlot;
    public Item MainWeaponSlot;

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
