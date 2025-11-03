using UnityEngine;

[DisallowMultipleComponent]
public class Highlighter : MonoBehaviour
{
    [Header("Glow")]
    public Color glowColor = Color.cyan;
    public float baseIntensity = 1.5f;
    public float pulseIntensity = 1.0f;
    public float pulseSpeed = 2.0f;

    Renderer[] rends;
    MaterialPropertyBlock mpb;
    float t0;

    void Awake()
    {
        rends = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        t0 = Time.time;
        ApplyEmission(1f);
    }

    void OnDisable()
    {
        foreach (var r in rends)
        {
            if (!r) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_EmissionColor", Color.black);
            r.SetPropertyBlock(mpb);
            r.material.DisableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        float s = baseIntensity + Mathf.Sin((Time.time - t0) * pulseSpeed * 2f * Mathf.PI) * pulseIntensity;
        ApplyEmission(Mathf.Max(0f, s));
    }

    void ApplyEmission(float intensity)
    {
        Color c = glowColor * Mathf.LinearToGammaSpace(intensity);
        foreach (var r in rends)
        {
            if (!r) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_EmissionColor", c);
            r.SetPropertyBlock(mpb);
            r.material.EnableKeyword("_EMISSION");
        }
    }
}
