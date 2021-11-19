using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Extensions;

public class EnemyAI : MonoBehaviour
{
    public LayerMask layerMask;

    private bool moveOnUpdate = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (moveOnUpdate)
        {
            MakeMove();
            moveOnUpdate = false;
        }
    }

    private void MakeMove()
    {
        var corner = transform.position;

        List<Vector2> hashMap = new List<Vector2>();
        hashMap.Add(new Vector2(-1, 1));
        hashMap.Add(new Vector2(0, 1));
        hashMap.Add(new Vector2(1, 1));

        hashMap.Add(new Vector2(-1, 0));
        hashMap.Add(new Vector2(1, 0));

        hashMap.Add(new Vector2(-1, -1));
        hashMap.Add(new Vector2(0, -1));
        hashMap.Add(new Vector2(1, -1));

        Vector2 target = Vector2.zero;

        while (hashMap.Count > 0)
        {
            target = hashMap[Random.Range(0, hashMap.Count - 1)];
            target += corner.ToVector2();

            if (!Physics2D.OverlapCircle(target, .1f, layerMask))
            {
                break;
            }
        }

        transform.position = target;
    }

    public void MakeTurn()
    {
        moveOnUpdate = true;
    }

    private void OnEnable()
    {
        TurnManager.turnManager.Register(this);
    }

    private void OnDisable()
    {
        // This can fail if the turn manager is killed first
        TurnManager.turnManager?.DeRegister(this);
    }
}
