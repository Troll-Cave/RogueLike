using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{
    public LayerMask blockers;

    private void Update()
    {
        var renderer = GetComponent<SpriteRenderer>();

        if (Physics2D.OverlapCircle(transform.transform.position, 0.1f, blockers))
        {
            renderer.color = renderer.color.Transparent();
        }
        else
        {
            renderer.color = renderer.color.Opaque();
        }
    }
}
