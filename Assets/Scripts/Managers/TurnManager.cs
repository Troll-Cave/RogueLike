using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles turns!
/// </summary>
public static class TurnManager
{
    // Start is called before the first frame update

    private static List<EnemyAI> _list = new List<EnemyAI>();

    public static event Action OnTurnEnd;

    public static List<string> messages = new List<string>();

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

    public static void AddMessage(string message)
    {
        messages.Insert(0, message);
        
        if (messages.Count > 5)
        {
            messages.RemoveAt(messages.Count - 1);
        }
    }
}
