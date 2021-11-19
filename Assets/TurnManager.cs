using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles turns!
/// </summary>
public class TurnManager : MonoBehaviour
{
    // Start is called before the first frame update

    private static TurnManager _turnManager;
    private List<EnemyAI> _list = new List<EnemyAI>();

    public TurnManager()
    {
        _turnManager = this;
    }

    public static TurnManager turnManager
    {
        get 
        {
            if (_turnManager == null)
            {
                //Debug.LogError("No turn manager");
                return null;
            }
            return _turnManager;
        }
    }

    public void RunTurns()
    {
        _list.ForEach(ai => ai.MakeTurn());
    }

    public void Register(EnemyAI ai)
    {
        _list.Add(ai);
    }

    public void DeRegister(EnemyAI ai)
    {
        _list.Remove(ai);
    }
}
