using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Helper
{
    public static class VectorHelper
    {
        public static Vector2 Absolute(Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector2 XPart(Vector2 vector)
        {
            return new Vector2(vector.x, 0);
        }

        public static Vector2 YPart(Vector2 vector)
        {
            return new Vector2(0, vector.y);
        }

        public static Vector2 OneMinusVector(Vector2 vector)
        {
            return new Vector2(1 - vector.x, 1 - vector.y);
        }

        public static Vector2 VectorMinusOne(Vector2 vector)
        {
            return new Vector2(vector.x - 1, vector.y - 1);
        }
    }
}
