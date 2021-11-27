using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally just for holding items
public class DropsHolder : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    private void Update()
    {
        if (items.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
