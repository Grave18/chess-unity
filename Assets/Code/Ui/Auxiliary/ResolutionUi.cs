using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Ui.Auxiliary
{
    public class ResolutionUi // : IEquatable<ResolutionUi>
    {
        public int Width { get; }
        public int Height { get; }

        public ResolutionUi(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator ResolutionUi(Resolution resolution)
        {
            return new ResolutionUi(resolution.width,resolution.height);
        }

        public static implicit operator Resolution(ResolutionUi resolutionUi)
        {
            return new Resolution { width = resolutionUi.Width, height = resolutionUi.Height };
        }
#endif

        public bool Equals(ResolutionUi other)
        {
            return Width == other?.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
#if UNITY_5_3_OR_NEWER
            return obj is ResolutionUi other && Equals(other);
#else
            var other = obj as ResolutionUi;
            return other != null && Equals(other);
#endif
        }

        public override int GetHashCode()
        {
#if UNITY_5_3_OR_NEWER
            return HashCode.Combine(Width, Height);
#else
            return Width.GetHashCode() * Height.GetHashCode();
#endif
        }
    }
}