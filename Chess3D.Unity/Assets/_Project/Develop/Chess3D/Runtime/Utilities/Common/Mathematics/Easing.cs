using UnityEngine;

namespace Chess3D.Runtime.Utilities.Common.Mathematics
{
    public static class Easing
    {
        public static float Generic(float x, EasingType type)
        {
            return type switch
            {
                EasingType.Linear => Linear(x),
                EasingType.InQuad => InQuad(x),
                EasingType.OutQuad => OutQuad(x),
                EasingType.InOutQuad => InOutQuad(x),
                EasingType.InCubic => InCubic(x),
                EasingType.OutCubic => OutCubic(x),
                EasingType.InOutCubic => InOutCubic(x),
                EasingType.InSine => InSine(x),
                EasingType.OutSine => OutSine(x),
                EasingType.InOutSine => InOutSine(x),
                EasingType.InExpo => InExpo(x),
                EasingType.OutExpo => OutExpo(x),
                EasingType.InOutExpo => InOutExpo(x),
                EasingType.InCirc => InCirc(x),
                EasingType.OutCirc => OutCirc(x),
                EasingType.InOutCirc => InOutCirc(x),
                _ => x
            };
        }

        public static float Linear(float x)
        {
            return x;
        }

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

        public static float InExpo(float x)
        {
            return x == 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
        }

        public static float OutExpo(float x)
        {
            return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        }

        public static float InOutExpo(float x)
        {
            return x == 0f || x == 1f
                ? x
                : x < 0.5f
                    ? Mathf.Pow(2f, 20f * x - 10f) / 2f
                    : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
        }

        public static float InCirc(float x)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
        }

        public static float OutCirc(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
        }

        public static float InOutCirc(float x)
        {
            return x < 0.5f
                ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f
                : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
        }
    }

    public enum EasingType
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc
    }
}