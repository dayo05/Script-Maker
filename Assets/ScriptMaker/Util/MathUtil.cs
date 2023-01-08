using UnityEngine;

namespace ScriptMaker.Util
{
    public static class VectorMethod
    {
        public static bool In(this float f, double a, double b)
            => a <= f && f <= b;

        public static Vector3 Mean(this (Vector3 v1, Vector3 v2) a)
            => new((a.v1.x + a.v2.x) / 2, (a.v1.y + a.v2.y) / 2, (a.v1.z + a.v2.z) / 2);

        public static Vector3 Eq(this (Vector3 v1, Vector3 v2) a)
            => new(a.v1.x * a.v2.x, a.v1.y * a.v2.y, a.v1.z * a.v2.z);
    }
}