using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

using Extensions;
using System.Linq;
using UnityEngine.InputSystem;
using System;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap floors;

    private TileBase tile;

    private void Start()
    {
        GetComponent<PlayerInput>().actions["Scroll"].performed += ctx => Scroll(ctx.ReadValue<float>());
    }

    private void Scroll(float v)
    {
        Debug.Log(v);
        Camera.main.orthographicSize -= Mathf.Clamp(v, -2, 2);
    }

    private void Awake()
    {
        tile = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/stone_2_15.asset");

        DrawMap();
    }

    private void DrawMap()
    {
        var map = new HashSet<Vector2>();
        map.Add(new Vector2(-1, -1));
        map.Add(new Vector2(0, 0));
        map.Add(new Vector2(1, 1));

        foreach (var entry in map)
        {
            floors.SetTile(entry.ToVector3int(), tile);
        }
    }
}
