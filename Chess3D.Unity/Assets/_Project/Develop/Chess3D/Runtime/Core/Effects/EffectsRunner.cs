using System.Collections;
using Chess3D.Runtime.Utilities.Common;
using Chess3D.Runtime.Utilities.Common.Mathematics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chess3D.Runtime.Core.Effects
{
    public sealed class EffectsRunner : MonoBehaviour
    {
        [SerializeField] private float dissolveTime = 1f;
        [SerializeField] private float highlightDurationSec = 0.1f;
        [SerializeField] private float unhighlightDurationSec = 0.06f;

        private static readonly int TimeId = Shader.PropertyToID("_T");
        private static readonly int SeedId = Shader.PropertyToID("_Seed");
        private static readonly int IsHighlightedId = Shader.PropertyToID("_IsHighlighted");
        private static readonly int HighlightT = Shader.PropertyToID("_HighlightT");

        private float hoverT = 1;
        private Coroutine _appearCoroutine;

        private MeshRenderer _meshRenderer;
        private Material _material;


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

        public async UniTask HighlightPiece(bool enableHighlight)
        {
            if (enableHighlight)
            {
                _material.SetInt(IsHighlightedId, 1);
                while (hoverT < 1)
                {
                    hoverT += Time.deltaTime/highlightDurationSec;
                    _material.SetFloat(HighlightT, Easing.Generic(hoverT, EasingType.InOutQuad));
                    await UniTask.NextFrame();
                }
            }
            else
            {
                while (hoverT > 0)
                {
                    hoverT -= Time.deltaTime/unhighlightDurationSec;
                    _material.SetFloat(HighlightT,Easing.Generic(hoverT, EasingType.InOutQuad));
                    await UniTask.NextFrame();
                }
                _material.SetInt(IsHighlightedId, 0);
            }
        }

        private void OnMouseEnter()
        {
            HighlightPiece(true).Forget();
        }

        private void OnMouseExit()
        {
            HighlightPiece(false).Forget();
        }
    }
}
