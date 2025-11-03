using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    public enum AxisMode { Uniform, X, Y, Z }
    [Header("Anim")]
    public AxisMode axis = AxisMode.Y; // Y boyunca uzayıp kısalsın
    public float amplitude = 0.15f;    // ± oran (0.15 => ±%15)
    public float speed = 1.6f;         // Hz (saniyedeki tur)
    public AnimationCurve ease = AnimationCurve.Linear(0,0, 1,1);
    bool enableAnimation = false;
    Vector3 baseScale;
    float t0;

    void Awake()
    {
        baseScale = transform.localScale;
        t0 = Time.time;
    }

    void OnEnable()
    {
        // tekrar enable edilince fazı sıfırla
        t0 = Time.time;
    }

    void OnDisable()
    {
        // kapatınca ölçeği eski haline çek
        transform.localScale = baseScale;
    }

    public void ToggleAnimation()
    {
        enableAnimation = !enableAnimation;
    }

    void Update()
    {
        if (!enableAnimation) return;

        float tau = (Time.time - t0) * speed;
        float s = Mathf.Sin(tau * Mathf.PI * 2f);         // -1..1
        s = ease.Evaluate((s + 1f) * 0.5f) * 2f - 1f;     // eğri ile yumuşat
        float k = 1f + (s * amplitude);                   // 1 ± amplitude

        Vector3 target = baseScale;
        switch (axis)
        {
            case AxisMode.Uniform: target *= k; break;
            case AxisMode.X: target = new Vector3(baseScale.x * k, baseScale.y, baseScale.z); break;
            case AxisMode.Y: target = new Vector3(baseScale.x, baseScale.y * k, baseScale.z); break;
            case AxisMode.Z: target = new Vector3(baseScale.x, baseScale.y, baseScale.z * k); break;
        }
        transform.localScale = target;
    }
}
