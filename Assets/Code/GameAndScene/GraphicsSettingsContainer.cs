using System.Linq;
using UnityEngine;

namespace GameAndScene
{
    public class GraphicsSettingsContainer : MonoBehaviour
    {
        private const string QualityKey = "Quality";
        private const string FullScreenModeKey = "FullScreenMode";
        private const string ResolutionWidthKey = "ResolutionWidth";
        private const string ResolutionHeightKey = "ResolutionHeight";

        private int _width;
        private int _height;
        private bool _isResolutionChanged;

        private int _fullScreenMode;
        private bool _isFullScreenModeChanged;

        private int _qualityIndex;
        private bool _isQualityChanged;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
            if (!PlayerPrefs.HasKey(QualityKey))
            {
                int quality = QualitySettings.GetQualityLevel();
                PlayerPrefs.SetInt(QualityKey, quality);
            }

            if (!PlayerPrefs.HasKey(FullScreenModeKey))
            {
                FullScreenMode fullScreenMode = Screen.fullScreenMode;
                PlayerPrefs.SetInt(FullScreenModeKey, (int)fullScreenMode);
            }

            if (!PlayerPrefs.HasKey(ResolutionWidthKey) || !PlayerPrefs.HasKey(ResolutionHeightKey))
            {
                int width = Screen.width;
                int height = Screen.height;

                PlayerPrefs.SetInt(ResolutionWidthKey, width);
                PlayerPrefs.SetInt(ResolutionHeightKey, height);
            }
        }

        public Resolution GetResolution()
        {
            int width = PlayerPrefs.GetInt(ResolutionWidthKey, 640);
            int height = PlayerPrefs.GetInt(ResolutionHeightKey, 480);

            return new Resolution{width = width, height = height};
        }

        public void SetResolution(Resolution resolution)
        {
            _width = resolution.width;
            _height = resolution.height;

            _isResolutionChanged = true;
        }

        public int GetFullScreenMode()
        {
            return PlayerPrefs.GetInt(FullScreenModeKey, 0);
        }

        public void SetFullScreenMode(int fullScreenMode)
        {
            _fullScreenMode = fullScreenMode;

            _isFullScreenModeChanged = true;
        }

        public int GetQualityIndex()
        {
            return _qualityIndex;
        }

        public string GetQualityString()
        {
            int quality = PlayerPrefs.GetInt(QualityKey, 0);
            string qualityName = QualitySettings.names[quality];

            return qualityName;
        }

        public void SetQuality(string quality)
        {
            _qualityIndex = QualitySettings.names.ToList().IndexOf(quality);
            _isQualityChanged = true;
        }

        public void SetQuality(int qualityIndex)
        {
            _qualityIndex = qualityIndex;
            _isQualityChanged = true;
        }

        public void ApplySettings()
        {
            if (_isResolutionChanged)
            {
                var fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(FullScreenModeKey, 0);

                Screen.SetResolution(_width, _height, fullScreenMode);
                PlayerPrefs.SetInt(ResolutionWidthKey, _width);
                PlayerPrefs.SetInt(ResolutionHeightKey, _height);

                _isResolutionChanged = false;
            }

            if (_isFullScreenModeChanged)
            {
                int width = PlayerPrefs.GetInt(ResolutionWidthKey, 0);
                int height = PlayerPrefs.GetInt(ResolutionHeightKey, 0);

                Screen.SetResolution(width, height, (FullScreenMode)_fullScreenMode);
                PlayerPrefs.SetInt(FullScreenModeKey, _fullScreenMode);

                _isFullScreenModeChanged = false;
            }

            if (_isQualityChanged)
            {
                QualitySettings.SetQualityLevel(_qualityIndex);
                PlayerPrefs.SetInt(QualityKey, _qualityIndex);

                _isQualityChanged = false;
            }
        }

        public bool IsNeedToApplySettings()
        {
            return _isResolutionChanged || _isFullScreenModeChanged || _isQualityChanged;
        }
    }
}