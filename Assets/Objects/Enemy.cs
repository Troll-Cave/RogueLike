using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Data/Enemy")]
public class Enemy : ScriptableObject
{
    public string spriteLetter;
    public Colors color;
}
