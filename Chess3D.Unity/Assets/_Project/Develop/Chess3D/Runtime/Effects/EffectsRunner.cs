using System.Collections;
using Chess3D.Runtime.UtilsCommon;
using UnityEngine;

namespace Chess3D.Runtime.Effects
{
    public class EffectsRunner : MonoBehaviour
    {
        [SerializeField] private float dissolveTime = 1f;

        private static readonly int TimeId = Shader.PropertyToID("_T");
        private static readonly int SeedId = Shader.PropertyToID("_Seed");
        private static readonly int IsHighlightedId = Shader.PropertyToID("_IsHighlighted");

        private MeshRenderer _meshRenderer;
        private Material _material;

        private Coroutine _appearCoroutine;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = _meshRenderer.material;
        }

        /// Disappear towards 1.0f
        public void ProgressDisappear(float t)
        {
            _appearCoroutine?.Stop();
            _material.SetFloat(TimeId, 1-t);
        }

        private void Start()
        {
            AddRandomnessToPattern();

            Appear();
        }

        private void AddRandomnessToPattern()
        {
            _material.SetFloat(SeedId, Random.Range(0f, 100f));
        }

        public void Appear()
        {
            _appearCoroutine?.Stop();
            _appearCoroutine = AppearRoutine(0.001f, 1f).RunCoroutine();
        }

        public void Disappear()
        {
            _appearCoroutine?.Stop();
            _appearCoroutine = AppearRoutine(1f, 0.001f).RunCoroutine();
        }

        private IEnumerator AppearRoutine(float start, float end)
        {
            float t = 0f;
            while (t <= dissolveTime)
            {
                t += Time.deltaTime;
                float calculatedTime = Mathf.Lerp(start, end, t / dissolveTime);
                _material.SetFloat(TimeId, calculatedTime);

                yield return null;
            }
        }

        public void HighlightPiece(bool enableHighlight)
        {
            _material.SetInt(IsHighlightedId, enableHighlight ? 1 : 0);
        }
    }
}
