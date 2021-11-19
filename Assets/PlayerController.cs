using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Extensions;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Move(Vector3 target)
    {
        var collider = Physics2D.OverlapCircle(target, .1f);
        

        var current = gameObject.transform.position;

        if (collider != null)
        {
            if (collider.gameObject.tag == "Enemies")
            {
                Debug.Log(collider.gameObject);
                Destroy(collider.gameObject);

                TurnManager.turnManager?.RunTurns(); // attacks are an action
                return;
            }

            var targetX = target.WithY(current.y);
            var targetY = target.WithX(current.x);

            if (Physics2D.OverlapCircle(targetX, .1f) == null)
            {
                target = targetX;
            }
            else if (Physics2D.OverlapCircle(targetY, .1f) == null)
            {
                target = targetY;
            }
            else
            {
                target = gameObject.transform.position;
            }
        }

        if (gameObject.transform.position != target)
        {
            gameObject.transform.position = target;
            TurnManager.turnManager?.RunTurns();
        }
        
    }
}
