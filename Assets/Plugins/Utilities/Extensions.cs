using UnityEngine;

namespace Assets.Plugins.Utilities
{
    public static class Extensions
    {
        public static Vector2 Random(this Vector2 vector, float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));
        }
        public static float MapClamped(this float s, float InMin, float InMax, float OutMin, float OutMax)
        {
            return Mathf.Clamp(OutMin + (s - InMin) * (OutMax - OutMin) / (InMax - InMin), Mathf.Min(OutMin,OutMax), Mathf.Max(OutMin, OutMax));
        }
    }
}
