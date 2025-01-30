using System;
using System.Collections;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float dissolveTime = 1f;

    private static readonly int TimeId = Shader.PropertyToID("_T");
    private static readonly int SeedId = Shader.PropertyToID("_Seed");
    private static readonly int IsHighlightedId = Shader.PropertyToID("_IsHighlighted");

    private MeshRenderer _meshRenderer;
    private Material _material;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
    }

    private void Start()
    {
        _material.SetFloat(SeedId, UnityEngine.Random.Range(0f, 100f));

        StartCoroutine(Appear());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(Appear());
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Disappear());
        }
    }

    private IEnumerator Appear()
    {
        float time = 0f;
        while (time <= dissolveTime)
        {
            time += Time.deltaTime;
            float calculatedTime = Mathf.Lerp(0.001f, 1f, time / dissolveTime);
            _material.SetFloat(TimeId, calculatedTime);
            yield return null;
        }
    }

    private IEnumerator Disappear()
    {
        float time = 0f;
        while (time <= dissolveTime)
        {
            time += Time.deltaTime;
            float calculatedTime = Mathf.Lerp(1f, 0.001f, time / dissolveTime);
            _material.SetFloat(TimeId, calculatedTime);
            yield return null;
        }
    }

    public void HighlightPiece(bool enableHighlight)
    {
        _material.SetInt(IsHighlightedId, enableHighlight ? 1 : 0);
    }
}
