using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Chess3D.Runtime.Logo
{
    public class Fader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Settings")]
        [SerializeField] private Image image;
        [SerializeField] private float secondsToStart = 1f;
        [SerializeField] private float secondsToAppear = 3f;
        [SerializeField] private float secondsToStay = 3f;
        [SerializeField] private float secondsToFade = 3f;
        [SerializeField] private float scaleFactor = 1f;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(secondsToStart);

            StartCoroutine(Scale());

            // Appear
            while (image.color.a < 1f)
            {
                var color = image.color;
                color.a += Time.unscaledDeltaTime/secondsToAppear;
                image.color = color;

                yield return null;
            }

            // Wait
            yield return new WaitForSeconds(secondsToStay);

            // Fade
            while (image.color.a > 0f)
            {
                var color = image.color;
                color.a -= Time.unscaledDeltaTime/secondsToFade;
                image.color = color;

                yield return null;
            }

            sceneLoader.LoadMainMenu();
        }

        private IEnumerator Scale()
        {
            float period = secondsToStart + secondsToAppear + secondsToStay + secondsToFade;
            while (period >= 0f)
            {
                float uDt = Time.unscaledDeltaTime;
                period -= uDt;

                Vector3 scale = image.transform.localScale;
                scale.x += uDt * scaleFactor;
                scale.y += uDt * scaleFactor;
                scale.z += uDt * scaleFactor;
                image.transform.localScale = scale;

                yield return null;
            }
        }
    }
}
