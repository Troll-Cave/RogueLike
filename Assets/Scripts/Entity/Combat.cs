using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public Dictionary<Stat, int> stats { get; set; } = new Dictionary<Stat, int>();
    public Dictionary<Stat, int> tempStats { get; set; } = new Dictionary<Stat, int>();
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
    public string description;
}