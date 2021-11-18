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

        gameObject.transform.position = target;
    }
}
