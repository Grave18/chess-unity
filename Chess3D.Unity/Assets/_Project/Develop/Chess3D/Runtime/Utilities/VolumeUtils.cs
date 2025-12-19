using UnityEngine;

namespace Chess3D.Runtime.Utilities
{
    public static class VolumeUtils
    {
        public static float Map01ToDb(float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1f);
            return Mathf.Log10(volume) * 20f;
        }
    }
}