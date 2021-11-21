using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles turns!
/// </summary>
public static class TurnManager
{
    // Start is called before the first frame update

    private static List<EnemyAI> _list = new List<EnemyAI>();

    public static void RunTurns()
    {
        _list.ForEach(ai => ai.MakeTurn());
    }

    public static void Register(EnemyAI ai)
    {
        _list.Add(ai);
    }

    public static void DeRegister(EnemyAI ai)
    {
        _list.Remove(ai);
    }
}
