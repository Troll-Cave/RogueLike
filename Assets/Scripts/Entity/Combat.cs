using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public Dictionary<Stat, int> stats { get; set; } = new Dictionary<Stat, int>();
    public List<Effect> effects { get; set; } = new List<Effect>();

    public int GetStat(Stat stat)
    {
        if (!stats.ContainsKey(stat))
        {
            stats[stat] = 0;
        }

        return stats[stat] + effects.Where(x => x.stat == stat).Select(x => x.change).Sum();
    }

    public void SetStats(int health, int mana, int strength, int dexterity, int knowledge, int defense)
    {
        stats[Stat.health] = health;
        stats[Stat.mana] = mana;
        stats[Stat.strength] = strength;
        stats[Stat.dexterity] = dexterity;
        stats[Stat.knowledge] = knowledge;
        stats[Stat.defense] = defense;
    }
}

public enum Stat
{
    health,
    mana,
    strength,
    dexterity,
    knowledge,
    defense,
}

public class Effect
{
    /// <summary>
    /// Key is used to uniquely identify effects
    /// </summary>
    public string key { get; set; }
    public string description { get; set; }
    public Stat stat { get; set; }
    public int change { get; set; }
    public float expiration { get; set; }

}