using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public ushort level { get; set; } = 1;
    public Dictionary<Stat, int> stats { get; set; } = new Dictionary<Stat, int>();
    public List<Effect> effects { get; set; } = new List<Effect>();

    /// <summary>
    /// This is triggered when damage is done so we can update the UI
    /// </summary>
    public Action updated;

    public void AddEffect()
    {

    }

    public int GetStat(Stat stat)
    {
        if (!stats.ContainsKey(stat))
        {
            stats[stat] = 0;
        }

        return stats[stat] + effects.Where(x => x.stat == stat).Select(x => x.change).Sum();
    }

    public string GetStatForUI(Stat stat)
    {
        if (!stats.ContainsKey(stat))
        {
            stats[stat] = 0;
        }

        var modifier = effects.Where(x => x.stat == stat).Select(x => x.change).Sum();
        var modifierSymbol = "";

        if (modifier != 0)
        {
            if (modifier > 0)
            {
                modifierSymbol = "+";
            }
            else
            {
                modifierSymbol = "-";
            }

            return $"{stats[stat] + modifier} ({modifierSymbol}{modifier})";
        }    
        else
        {
            return stats[stat].ToString();
        }
    }

    public void SetStats(int health, int mana, int strength, int dexterity, int knowledge, int defense)
    {
        stats[Stat.maxHealth] = health;
        stats[Stat.health] = health;
        stats[Stat.mana] = mana;
        stats[Stat.strength] = strength;
        stats[Stat.dexterity] = dexterity;
        stats[Stat.knowledge] = knowledge;
        stats[Stat.defense] = defense;
        stats[Stat.fullness] = 10;
    }

    public void Attack(Combat target, Stat attackType, int maxHit)
    {
        TurnManager.CombatHappened();

        var roll = UnityEngine.Random.Range(1, 20);

        var modifier = GetModifier(GetStat(attackType));

        if ((roll + modifier) > target.GetStat(Stat.defense))
        {
            var damage = UnityEngine.Random.Range(1, maxHit);
            var myColor = ColorUtility.ToHtmlStringRGBA(gameObject.GetComponent<SpriteRenderer>().color);
            var targetColor = ColorUtility.ToHtmlStringRGBA(target.gameObject.GetComponent<SpriteRenderer>().color);
            TurnManager.AddMessage($"<color=#{myColor}>{name}</color> hit <color=#{targetColor}>{target.name}</color> for <b>{damage}</b> damage");
            target.Hit(damage);
        }
    }

    private void Hit(int damage)
    {
        stats[Stat.health] -= damage;

        if (updated != null)
        {
            updated();
        }

        if (stats[Stat.health] < 1)
        {
            var myColor = ColorUtility.ToHtmlStringRGBA(gameObject.GetComponent<SpriteRenderer>().color);
            TurnManager.AddMessage($"<color=#{myColor}>{name}</color> has died");
            //Destroy(gameObject);
        }
    }

    public int GetModifier(int stat)
    {
        if (stat == 10)
        {
            return 0;
        }
        else if (stat > 10)
        {
            return Mathf.CeilToInt(((float)stat - 10) / 3);
        }
        else
        {
            return Mathf.FloorToInt(((float)stat - 10) / 3);
        }
    }
}

public enum Stat
{
    maxHealth,
    health,
    mana,
    strength,
    dexterity,
    knowledge,
    defense,
    fullness,
}

public struct Effect
{
    /// <summary>
    /// Key is used to uniquely identify effects
    /// </summary>
    public string key { get; set; }
    public string description { get; set; }
    public Stat stat { get; set; }
    public int change { get; set; }
    public EffectSource source { get; set; }
    public float expiration { get; set; }

    public Effect(string key, string description, Stat stat, int change, EffectSource source = EffectSource.equipment, float expiration = 0)
    {
        this.key = key;
        this.description = description;
        this.stat = stat;
        this.change = change;
        this.expiration = expiration;
        this.source = source;
    }
}

public enum EffectSource
{
    equipment,
}