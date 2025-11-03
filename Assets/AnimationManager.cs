using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] SelectionManager selection;   // Inspector’dan sürükle (XR Origin üzerindeki)
    [SerializeField] bool addIfMissing = true;     // Seçili objede yoksa ScalePulse ekle

    void Reset()
    {
        if (!selection) selection = FindObjectOfType<SelectionManager>();
    }

    // UI Button → OnClick: Toggle
    public void ToggleSelectedPulse()
    {
        var pulse = GetPulseOfCurrent();
        if (!pulse) return;
        pulse.ToggleAnimation();
    }

    // — helper —
    ScalePulse GetPulseOfCurrent()
    {
        if (!selection || !selection.current) return null;

        // Seçili hedefte veya çocuklarında ara
        var pulse = selection.current.GetComponent<ScalePulse>()
                 ?? selection.current.GetComponentInChildren<ScalePulse>(true);

        if (!pulse && addIfMissing)
            pulse = selection.current.gameObject.AddComponent<ScalePulse>();

        return pulse;
    }
}
