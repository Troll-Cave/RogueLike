using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Data/Enemy")]
public class Enemy : ScriptableObject
{
    public string spriteLetter;
    public Colors color;

    public int health;
    public int mana;
    public int strength;
    public int dexterity;
    public int knowledge;
    public int defense;

    public int maxHit;

    public DropTable dropTable;
}
