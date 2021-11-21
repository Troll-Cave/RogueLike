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

    private const int maxMapSize = 50;

    private void Start()
    {
        GetComponent<PlayerInput>().actions["Scroll"].performed += ctx => Scroll(ctx.ReadValue<float>());
    }

    private void Scroll(float v)
    {
        Camera.main.orthographicSize -= Mathf.Clamp(v, -2, 2);
        if (Camera.main.orthographicSize < 0)
        {
            Camera.main.orthographicSize = 0;
        }
    }

    private void Awake()
    {
        tile = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/stone_2_15.asset");

        var t = Time.realtimeSinceStartup;
        DrawMap();
        Debug.Log(Time.realtimeSinceStartup - t);
    }

    private void DrawMap()
    {
        var map = new HashSet<Vector2>();
        
        var rooms = GetRooms();
        
        foreach (var room in rooms)
        {
            foreach (var v in room.GetVectors())
            {
                map.Add(v);
            }
        }

        AddConnections(map, rooms);

        foreach (var entry in map)
        {
            floors.SetTile(entry.ToVector3int(), tile);
        }
    }

    private void AddConnections(HashSet<Vector2> map, List<Room> rooms)
    {
        var allRooms = new List<Room>(rooms);
        var remainingRooms = new List<Room>(rooms);

        var weights = new Dictionary<string, float>();

        // Anything here is connected
        var treeNodes = new List<RoomTreeNode>();

        var breaker = 0;

        var root = remainingRooms.First();

        while ((root != null) && (breaker++ < 500))
        {
            remainingRooms.Remove(root);

            RoomTreeNode node = GetTreeNode(root, weights, allRooms);
            treeNodes.Add(node);

            // I have litterally not done a linq query in 4 years holy shit
            var query = from t in node.NearestNeighbors
                        join r in remainingRooms on t equals r.id
                        select r;

            var target = query.FirstOrDefault();

            if (target == null)
            {
                Room newTarget = null;

                foreach (var nodeInTree in treeNodes)
                {

                    var nQuery = from neighbor in nodeInTree.NearestNeighbors
                                 join r in remainingRooms on neighbor equals r.id
                                 select r;

                    newTarget = nQuery.FirstOrDefault();

                    if (newTarget != null)
                    {
                        root = allRooms.First(r => r.id == nodeInTree.id);
                        target = newTarget;
                        break;
                    }
                }

                // If it gets here, no possible neighbors
                if (newTarget == null)
                {
                    break;
                }
            }

            AddConnection(root, target, map);
            root = target;
        }

        Debug.Log(remainingRooms.Count);
    }

    private RoomTreeNode GetTreeNode(Room root, Dictionary<string, float> allWeights, List<Room> allRooms)
    {
        var retval = new RoomTreeNode();
        retval.id = root.id;

        var weights = new List<Tuple<int, float>>();

        foreach (var room in allRooms)
        {
            if (room.id == root.id)
            {
                continue;
            }

            var weightKey = $"{Math.Min(root.id, room.id)}-{Math.Max(root.id, room.id)}";

            if (allWeights.ContainsKey(weightKey))
            {
                weights.Add(new Tuple<int, float>(room.id, allWeights[weightKey]));
            }
            else
            {
                var weight = (Mathf.Abs(root.x - room.x) + Mathf.Abs(root.y - room.y)) / 2f;
                weights.Add(new Tuple<int, float>(room.id, weight));
                allWeights[weightKey] = weight;
            }
        }

        var orderedWeights = weights.OrderBy(x => x.Item2).ToList();

        retval.NearestNeighbors = orderedWeights
            .Select(w => w.Item1)
            .Take(3)
            .ToList();

        return retval;
    }

    private void AddConnection(Room room1, Room room2, HashSet<Vector2> map)
    {
        var tile1 = room1.GetRandomPoint();
        var tile2 = room2.GetRandomPoint();

        var verticleChange = tile1.y < tile2.y ? 1 : -1;
        var horizontalChange = tile1.x < tile2.x ? 1 : -1;

        var currentPoint = tile1;

        Action moveHorizontal = () =>
        {
            var breaker = 0;

            while (currentPoint.x != tile2.x && (breaker++ <= 500))
            {
                currentPoint = currentPoint.AddX(horizontalChange);
                map.Add(currentPoint);
            }
        };

        Action moveVertical = () =>
        {
            var breaker = 0;
            while (currentPoint.y != tile2.y && (breaker++ <= 500))
            {
                currentPoint = currentPoint.AddY(verticleChange);
                map.Add(currentPoint);
            }
        };

        if (UnityEngine.Random.Range(0, 1) == 0)
        {
            moveHorizontal();
            moveVertical();
        }
        else
        {
            moveVertical();
            moveHorizontal();
        }
    }

    private List<Room> GetRooms()
    {
        var collisionMap = new HashSet<Vector2>();
        var rooms = new List<Room>();

        var roomCount = Math.Pow(maxMapSize * 2 / 10, 2);

        for (int i = 0; i < roomCount; i++)
        {
            var goodToGo = true;
            
            for (int retry = 0; retry < 5; retry++)
            {
                var newCollisionMap = new HashSet<Vector2>(collisionMap);

                var room = new Room(i);

                goodToGo = true;

                foreach (var point in room.GetPhatVectors())
                {
                    if (newCollisionMap.Contains(point))
                    {
                        goodToGo = false;
                        break;
                    }
                }

                if (!goodToGo)
                {
                    continue;
                }

                // If we are here we should be good

                rooms.Add(room);

                foreach (var point in room.GetPhatVectors())
                {
                    collisionMap.Add(point);
                }

                break;
            }

            if (!goodToGo)
            {
                break;
            }
        }

        return rooms;
    }

    class RoomTreeNode
    {
        public int id { get; set; }
        public List<int> NearestNeighbors { get; set; } = new List<int>();
        public List<int> Connections { get; set; } = new List<int>();
    }

    class Room
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public int height { get; set; } = UnityEngine.Random.Range(3, 5);
        public int width { get; set; } = UnityEngine.Random.Range(3, 5);

        public Room(int id)
        {
            this.id = id;
            x = UnityEngine.Random.Range(-maxMapSize, maxMapSize);
            y = UnityEngine.Random.Range(-maxMapSize, maxMapSize);
        }

        public List<Vector2> GetVectors()
        {
            var retval = new List<Vector2>();

            for (var x = this.x; x < (this.x + width); x++)
            {
                for (var y = this.y; y < (this.y + height); y++)
                {
                    retval.AddVector(x, y);
                }
            }

            return retval;
        }

        /// <summary>
        /// This is for handling collision for rooms
        /// </summary>
        /// <returns></returns>
        public List<Vector2> GetPhatVectors()
        {
            var retval = new List<Vector2>();

            var alteredX = x - 1;
            var alteredY = y - 1;

            var alteredHeight = height + 2;
            var alteredWidth = width + 2;

            for (var x = alteredX; x < (alteredX + alteredWidth); x++)
            {
                for (var y = alteredY; y < (alteredY + alteredHeight); y++)
                {
                    retval.AddVector(x, y);
                }
            }

            return retval;
        }

        /// <summary>
        /// Get a random point in the room
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRandomPoint()
        {
            return new Vector2(UnityEngine.Random.Range(x, x + width - 1), UnityEngine.Random.Range(y, y + height - 1));
        }
    }
}
