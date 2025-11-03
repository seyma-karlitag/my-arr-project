using UnityEngine;

[DisallowMultipleComponent]
public class SelectableTarget : MonoBehaviour
{
    [Tooltip("Seçildiğinde uygulanacak ölçek çarpanı")]
    public float selectedScaleMultiplier = 1.05f;

    Vector3 _baseLocalScale;
    bool _selected;

    void Awake()
    {
        _baseLocalScale = transform.localScale;
    }

    public void Select()
    {
        if (_selected) return;
        _selected = true;
        Debug.Log($"SelectableTarget selected: {name}", this);
        // Basit görsel feedback: sadece bu objeyi şişir
        transform.localScale = _baseLocalScale * selectedScaleMultiplier;
    }

    public void Deselect()
    {
        if (!_selected) return;
        _selected = false;

        // Ölçeği eski hâline getir
        transform.localScale = _baseLocalScale;
    }
}
