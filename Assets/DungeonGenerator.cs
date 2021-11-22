using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

using Extensions;
using System.Linq;
using UnityEngine.InputSystem;
using System;
using UnityEngine.U2D;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap floors;
    public Tilemap wallsTilemap;
    public GameObject enemyPrefab;

    public GameObject wallPrefab;
    public GameObject floorPrefab;

    public Transform playerTransform;

    private Tile tile;
    private List<Sprite> brokenFloors = new List<Sprite>();

    private const int maxMapSize = 10;

    private Tile wallTile;

    // These are constants for getting walls
    static Vector2 TopLeft = new(-1 , 1);
    static Vector2 Top = new(0, 1);
    static Vector2 TopRight = new(1, 1);

    static Vector2 Left = new(-1, 0);
    static Vector2 Right = new(1, 0);

    static Vector2 BottomLeft = new(-1, -1);
    static Vector2 Botton = new(0, -1);
    static Vector2 BottomRight = new(1, -1);

    private List<Enemy> enemies;

    private List<Vector2> getRelativeVectors()
    {
        var retval = new List<Vector2>();

        retval.Add(TopLeft);
        retval.Add(Top);
        retval.Add(TopRight);

        retval.Add(Left);
        retval.Add(Right);

        retval.Add(BottomLeft);
        retval.Add(Botton);
        retval.Add(BottomRight);

        return retval;
    }

    private void Start()
    {
        //GetComponent<PlayerInput>().actions["Scroll"].performed += ctx => Scroll(ctx.ReadValue<float>());
        
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
        enemies = Resources.LoadAll<Enemy>("Enemies/").ToList();

        Debug.Log(enemies);

        var atlas = Resources.Load<SpriteAtlas>("Sprites");

        tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        tile.sprite = atlas.GetSprite("rl_floor");
        tile.color = new Color32(95, 87, 79, 0xff);

        for (var i = 0; i < 4; i++)
        {
            brokenFloors.Add(atlas.GetSprite("rl_floor_broken_" + (i + 1)));
        }
        

        wallTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        wallTile.sprite = atlas.GetSprite("rl_wall");
        wallTile.color = new Color32(194, 195, 199, 0xff);

        var t = Time.realtimeSinceStartup;
        DrawMap();
        Debug.Log(Time.realtimeSinceStartup - t);
    }

    private void DrawMap()
    {
        var map = new HashSet<Vector2>();
        
        var rooms = GetRooms();

        rooms = AddConnections(map, rooms);

        foreach (var room in rooms)
        {
            foreach (var v in room.GetVectors())
            {
                map.Add(v);
            }
        }

        foreach (var entry in map)
        {
            floorPrefab.GetComponent<SpriteRenderer>().color = ColorsManager.DarkGray.Transparent();
            var floor = Instantiate(floorPrefab, entry.ToVector3int() + Vector3.up + Vector3.right, Quaternion.identity);

            if (UnityEngine.Random.Range(0, 5) == 0)
            {
                // We've got a match
                var renderer = floor.GetComponent<SpriteRenderer>();
                renderer.sprite = brokenFloors[UnityEngine.Random.Range(0, 3)];
            }
        }

        var walls = new HashSet<Wall>();

        var relativeVectors = getRelativeVectors();

        foreach (var floor in map)
        {
            foreach (var vector in relativeVectors)
            {
                var loc = floor + vector;
                if (!map.Contains(loc))
                {
                    // Got a match for a wall
                    var wall = walls.FirstOrDefault(x => x.position == loc) ?? new Wall(loc);
                    wall.floors.Add(Vector2.zero - vector);

                    walls.Add(wall);
                }
            }
        }

        foreach (var wall in walls)
        {
            var index = wall.GetWallTileIndex();
            if (index.HasValue)
            {
                wallPrefab.GetComponent<SpriteRenderer>().color = ColorsManager.LightGray.Transparent();
                var wallObject = Instantiate(wallPrefab, wall.position.ToVector3int() + Vector3.up + Vector3.right, Quaternion.identity);
            }
        }

        playerTransform.position = GetRandomRoom(rooms).GetRandomPointInWorld().ToVector3();

        foreach(var enemy in enemies)
        {
            enemyPrefab.GetComponent<EnemyAI>().enemy = enemy;
            enemyPrefab.GetComponent<SpriteRenderer>().color = ColorsManager.GetColor(enemy.color).Transparent();

            // Add some random enemies
            Instantiate(enemyPrefab, GetRandomRoom(rooms).GetRandomPointInWorld().ToVector3(), Quaternion.identity);
        }

        
        
    }

    Room GetRandomRoom(List<Room> rooms)
    {
        var i = UnityEngine.Random.Range(0, rooms.Count - 1);

        return rooms[i];
    }

    private List<Room> AddConnections(HashSet<Vector2> map, List<Room> rooms)
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

        // Return back just the rooms that were added
        return treeNodes.Select(r => r.id)
            .ToList()
            .Distinct()
            .Select(x => allRooms.FirstOrDefault(y => y.id == x))
            .ToList();
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

    class Wall
    {
        public Vector2 position { get; set; }
        public List<Vector2> floors { get; set; } = new List<Vector2>();

        public Wall(Vector2 position)
        {
            this.position = position;
        }

        public int? GetWallTileIndex()
        {
            if (floors.Contains(Top) && floors.Contains(Right) && floors.Contains(Left))
            {
                return 5;
            }

            if (floors.Contains(Botton) && floors.Contains(Right) && floors.Contains(Left))
            {
                return 6;
            }

            if (floors.Contains(Top) && floors.Contains(Botton) && floors.Contains(Left))
            {
                return 1;
            }

            if (floors.Contains(Top) && floors.Contains(Right))
            {
                return 2;
            }

            if (floors.Contains(Top) && floors.Contains(Left))
            {
                return 0;
            }

            if (floors.Contains(Botton) && floors.Contains(Right))
            {
                return 11;
            }

            if (floors.Contains(Botton) && floors.Contains(Left))
            {
                return 10;
            }

            if (floors.Contains(Right) && floors.Contains(BottomLeft))
            {
                return 9;
            }

            if (floors.Contains(Botton))
            {
                return 1;
            }

            if (floors.Contains(Top))
            {
                return 1;
            }

            if (floors.Contains(Right) || floors.Contains(Left))
            {
                return 5;
            }

            if (floors.Contains(BottomRight))
            {
                return 0;
            }

            if (floors.Contains(BottomLeft))
            {
                return 2;
            }

            if (floors.Contains(TopRight))
            {
                return 10;
            }

            if (floors.Contains(TopLeft))
            {
                return 11;
            }


            return null;
        }

        private List<Vector2> from(params Vector2[] vectors)
        {
            return vectors.ToList();
        }

        private bool CheckFloors(List<Vector2> vectors)
        {
            if (floors.Count != vectors.Count)
            {
                return false;
            }

            foreach(var vector in vectors)
            {
                if (!floors.Contains(vector))
                {
                    return false;
                }
            }

            return true;
        }
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
            var v = new Vector2(UnityEngine.Random.Range(x, x + (width)), UnityEngine.Random.Range(y, y + (height)));
            return v;
        }

        /// <summary>
        /// For some reason the grid is offset by one
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRandomPointInWorld()
        {
            return GetRandomPoint() + new Vector2(1, 1);
        }
    }
}
