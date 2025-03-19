using UnityEngine;

namespace Utils.Mathematics
{
    public static class Easing
    {
        public static float InQuad(float x)
        {
            return x * x;
        }

        public static float OutQuad(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }

        public static float InOutQuad(float x)
        {
            return x < 0.5f
                ? 2f * x * x
                : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
        }

        public static float InCubic(float x)
        {
            return x * x * x;
        }

        public static float OutCubic(float x)
        {
            return 1 - Mathf.Pow(1f - x, 3f);
        }

        public static float InOutCubic(float x)
        {
            return x < 0.5
                ? 4 * x * x * x
                : 1 - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
        }

        public static float InSine(float x)
        {
            return 1 - Mathf.Cos(x * Mathf.PI / 2f);
        }

        public static float OutSine(float x)
        {
            return Mathf.Sin(x * Mathf.PI / 2f);
        }

        public static float InOutSine(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }
    }
}