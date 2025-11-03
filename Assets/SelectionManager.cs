using UnityEngine;
using UnityEngine.InputSystem;   // yeni Input System
using UnityEngine.EventSystems;  // UI filtresi

public class SelectionManager : MonoBehaviour
{
    [Header("Ray ayarları")]
    public Camera viewCamera;
    public LayerMask selectableMask = ~0;
    public float rayMaxDistance = 20f;

    [Header("Durum")]
    public SelectableTarget current;      // şu an seçili
    public SelectableTarget lastSelected; // en son seçilen (history)

    void Reset()
    {
        if (!viewCamera) viewCamera = Camera.main;
    }

    public void moveEnabled()
    {
        if (current == null)
        {
            Debug.LogWarning("moveEnabled: No current selection.");
            return;
        }

        var floatUpDown = current.GetComponent<FloatUpDown>();
        if (floatUpDown == null)
            floatUpDown = current.GetComponentInChildren<FloatUpDown>();

        if (floatUpDown == null)
        {
            Debug.LogWarning($"moveEnabled: FloatUpDown not found on {current.name}.");
            return;
        }

        if (!floatUpDown.enabled)
        {
            floatUpDown.enabled = true;
            Debug.Log($"moveEnabled: Enabled FloatUpDown on {current.name}.");
        }
        else
        {
            Debug.Log($"moveEnabled: FloatUpDown already enabled on {current.name}.");
        }

        floatUpDown.setMove();
        Debug.Log($"moveEnabled: setMove called on {current.name}.");
    }
    void Update()
    {
        if (!InputHelpers.TryGetScreenTap(out var screenPos, ignoreUI: true))
            return;

        if (!viewCamera) return;

        var ray = viewCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, rayMaxDistance, selectableMask, QueryTriggerInteraction.Ignore))
        {
            var target = hit.collider.GetComponentInParent<SelectableTarget>();
            if (target)
            {
                if (current && current != target)
                    current.Deselect();

                // history: güncelle
                if (current != null)
                    lastSelected = current;

                current = target;
                current.Select();
                return;
            }
        }
        else
        {
            Debug.Log($"Cannot hit any object");
        }

        // Boşluğa tıklama → sadece current'ı kapat, lastSelected olduğu gibi kalsın
        if (current)
        {
            lastSelected = current; // son görülen seçim bu olsun
            current.Deselect();
            current = null;
        }
    }

    // İstersen dışarıdan çağırmak için yardımcılar:
    public void ReselectLast()
    {
        if (lastSelected == null) return;
        if (current && current != lastSelected) current.Deselect();
        current = lastSelected;
        current.Select();
    }

    public void ClearSelection(bool clearHistory = false)
    {
        if (current) current.Deselect();
        current = null;
        if (clearHistory) lastSelected = null;
    }
}
