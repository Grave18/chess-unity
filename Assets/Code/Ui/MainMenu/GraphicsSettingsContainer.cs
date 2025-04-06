using UnityEngine;

namespace Ui.MainMenu
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

        private int _quality;
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

        public void SetQuality(int quality)
        {
            _quality = quality;
            _isQualityChanged = true;
        }

        public void SetResolution(int width, int height)
        {
            _width = width;
            _height = height;

            _isResolutionChanged = true;
        }

        public void SetFullScreenMode(int fullScreenMode)
        {
            _fullScreenMode = fullScreenMode;

            _isFullScreenModeChanged = true;
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
                QualitySettings.SetQualityLevel(_quality);
                PlayerPrefs.SetInt(QualityKey, _quality);

                _isQualityChanged = false;
            }
        }

        public (int width, int height) GetResolution()
        {
            int width = PlayerPrefs.GetInt(ResolutionWidthKey, 0);
            int height = PlayerPrefs.GetInt(ResolutionHeightKey, 0);

            return (width, height);
        }

        public int GetFullScreenMode()
        {
            return PlayerPrefs.GetInt(FullScreenModeKey, 0);
        }

        public int GetQuality()
        {
            return PlayerPrefs.GetInt(QualityKey, 0);
        }
    }
}