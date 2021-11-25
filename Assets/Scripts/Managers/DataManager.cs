using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class DataManager
{
    public static SaveData saveData { get; set; } = null;

    public static void Save()
    {

    }

    public static void Load()
    {

    }
}

[Serializable]
public class SaveData
{
    public int experience { get; set; } = 1;
    public int level { get; set; } = 1;
    public int fullness { get; set; } = 0;
    public Dictionary<Stat, int> stats { get; set; } = new Dictionary<Stat, int>();
    public List<Effect> effects { get; set; } = new List<Effect>();
}