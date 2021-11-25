using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles turns!
/// 
/// Calmness happens after 4 turns out of combat
/// </summary>
public static class TurnManager
{
    // Start is called before the first frame update

    private static List<EnemyAI> _list = new List<EnemyAI>();

    public static List<string> messages = new List<string>();
    private static int turn = 0;

    private static int nextCalmTurn = 0;

    public static void RunTurns()
    {
        turn++;
        _list.ForEach(ai => ai.MakeTurn());
    }

    public static void CombatHappened()
    {
        nextCalmTurn = turn + 6;
    }

    public static bool IsCalm()
    {
        return turn >= nextCalmTurn;
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
