using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Extensions;

public class PlayerController : MonoBehaviour
{
    public LayerMask fowMask;
    public LayerMask obsticals;
    // Start is called before the first frame update
    void Start()
    {
        RevealFOW();
    }

    public void RevealFOW()
    {
        var fowColliders = Physics2D.OverlapCircleAll(transform.position, 3, fowMask);
        foreach (var fowCollider in fowColliders)
        {
            var renderer = fowCollider.GetComponent<SpriteRenderer>();
            renderer.color = renderer.color.Opaque();

            if (renderer.gameObject.name.StartsWith("Floor"))
            {
                Destroy(renderer.gameObject.GetComponent<BoxCollider2D>());
            }
        }
    }

    public void Move(Vector3 target)
    {
        var collider = Physics2D.OverlapCircle(target, .1f, obsticals);

        var current = gameObject.transform.position;

        if (collider != null)
        {
            Debug.Log(collider);
            if (collider.gameObject.tag == "Enemies")
            {
                Debug.Log(collider.gameObject);
                Destroy(collider.gameObject);

                TurnManager.RunTurns(); // attacks are an action
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

            RevealFOW();

            TurnManager.RunTurns();
        }
        
    }
}
