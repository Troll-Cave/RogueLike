using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Drop Table", menuName = "Data/Drop Table")]
public class DropTable : ScriptableObject
{
    public List<Drop> drops;
    
    public Item GetItem()
    {
        foreach (Drop drop in drops)
        {
            if (UnityEngine.Random.Range(1, 100) < drop.chance)
            {
                return drop.item;
            }
        }
        return null;
    }
}

[Serializable]
public struct Drop
{
    public int chance;
    public Item item;
}