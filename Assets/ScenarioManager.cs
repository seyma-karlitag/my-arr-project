using UnityEngine;
using UnityEngine.Events;

public class ScenarioManager : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] Transform scenario1Root;   // Sceneario1Root / Scenario1Root
    [SerializeField] Transform scenario2Root;   // Scenario2Root

    [Header("Startup")]
    [SerializeField] bool selectScenario1OnStart = true;

    [Header("Events")]
    public UnityEvent<int> onScenarioChanged;   // 1 veya 2 gönderir

    public int Current { get; private set; } = 1; // 0=none, 1=sc1, 2=sc2

    void Awake()
    {
        // Otomatik bul (isteğe bağlı). Inspector’dan atarsan gerek yok.
        if (!scenario1Root) scenario1Root = FindChildByNames(transform, "Sceneario1Root", "Scenario1Root");
        if (!scenario2Root) scenario2Root = FindChildByNames(transform, "Scenario2Root");
        Current = 1;
    }

    // UI Button → OnClick() buna bağla
    public void Toggle()
    {

        if (Current == 1)
        {
            scenario1Root.gameObject.SetActive(false);
            scenario2Root.gameObject.SetActive(true);
            Current = 2;
        }
        else if (Current == 2)
        {
            scenario1Root.gameObject.SetActive(true);
            scenario2Root.gameObject.SetActive(false);
            Current = 1;
        }
        onScenarioChanged?.Invoke(Current);
        Debug.Log($"[ScenarioManager] Active => Scenario {Current}");
    }

    public void ActivateScenario(int scenarioNumber)
    {
        if (scenarioNumber == 1)
        {
            scenario1Root.gameObject.SetActive(true);
            scenario2Root.gameObject.SetActive(false);
            Current = 1;
        }
        else if (scenarioNumber == 2)
        {
            scenario1Root.gameObject.SetActive(false);
            scenario2Root.gameObject.SetActive(true);
            Current = 2;
        }
        onScenarioChanged?.Invoke(Current);
        Debug.Log($"[ScenarioManager] Active => Scenario {Current}");
    }
    // Küçük yardımcı: birden çok olası adı dene
    Transform FindChildByNames(Transform parent, params string[] names)
    {
        foreach (var n in names)
        {
            var t = parent.Find(n);
            if (t) return t;
        }
        // Derin arama (isim eşleşmesi)
        foreach (Transform c in parent)
        {
            var t = FindChildByNames(c, names);
            if (t) return t;
        }
        return null;
    }
}
