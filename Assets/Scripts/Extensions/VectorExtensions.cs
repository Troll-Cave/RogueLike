using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 Round(this Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }

        public static Vector3 Clamp(this Vector3 vector, float min = -1, float max = 1)
        {
            // don't need to clamp z and no-one gives a shit about it
            return new Vector3(Mathf.Clamp(vector.x, min, max), Mathf.Clamp(vector.y, min, max), vector.z);
        }

        public static Vector3 ToVector3(this Vector2 vector, float z = 0)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3Int ToVector3int(this Vector2 vector, int z = 0)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), z);
        }

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static void AddVector(this ICollection<Vector2> list, int x, int y)
        {
            list.Add(new Vector2(x, y));
        }

        public static Vector2 AddX(this Vector2 vector, float x)
        {
            var newVector = vector;
            newVector.x += x;
            return newVector;
        }

        public static Vector2 AddY(this Vector2 vector, float y)
        {
            var newVector = vector;
            newVector.y += y;
            return newVector;
        }
    }
}
