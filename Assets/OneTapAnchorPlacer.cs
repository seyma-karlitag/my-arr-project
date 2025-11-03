using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;                 // New Input System
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class OneTapAnchorPlacer : MonoBehaviour
{
    [Header("AR Managers (XR Origin üzerinde)")]
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARAnchorManager  anchorManager;
    [SerializeField] ARPlaneManager   planeManager;

    [Header("Scene Roots / Prefab")]
    [SerializeField] Transform worldRoot;      // Tüm sanal içerik bunun altında

    static readonly List<ARRaycastHit> hits = new();
    public ScenarioManager scenarioManager;
    ARAnchor anchor;
    bool armed = false;
    bool placed = false;
    bool busy = false;

    public void ArmPlacement()
    {
        Debug.Log("[Placement] Armed. Dokunarak yerleştirebilirsin.");
        if (placed) return;
        armed = true;
    }

    void Update()
    {
        if (!armed || placed || busy) return;

        if (InputHelpers.TryGetScreenTap(out var screenPos, ignoreUI: true))
        {
            // Raycast → ilk hit
            if (!raycastManager || !raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
                return;

            var hit = hits[0];
            _ = PlaceAtHitAsync(hit); // fire-and-forget (busy flag koruyor)
        }
    }

    async Task PlaceAtHitAsync(ARRaycastHit hit)
    {
        Debug.Log("[Placement] Yerleştiriliyor...");
        busy = true;

        if (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.LogWarning("[Placement] AR henüz trackingte değil.");
            busy = false;
            return;
        }

        var pose = hit.pose;
        ARAnchor newAnchor = null;

        if (hit.trackable is ARPlane plane &&
            anchorManager != null &&
            anchorManager.descriptor != null &&
            anchorManager.descriptor.supportsTrackableAttachments)
        {
            newAnchor = anchorManager.AttachAnchor(plane, pose);
        }

        if (newAnchor == null && anchorManager != null)
        {
            var result = await anchorManager.TryAddAnchorAsync(pose);
            if (result.status.IsSuccess())
                newAnchor = result.value;
        }

        if (newAnchor == null)
        {
            Debug.LogWarning("[Placement] Anchor oluşturulamadı.");
            busy = false;
            return;
        }

        anchor = newAnchor;

        worldRoot.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
        worldRoot.SetParent(anchor.transform, worldPositionStays: true);
        worldRoot.gameObject.SetActive(true);
        scenarioManager.ActivateScenario(1); // varsayılan senaryo
        if (planeManager != null)
        {
            planeManager.requestedDetectionMode = PlaneDetectionMode.None;
            foreach (var p in planeManager.trackables)
                p.gameObject.SetActive(false);
        }

        placed = true;
        armed  = false;
        busy   = false;
        Debug.Log("[Placement] Tamamlandı. Anchor kuruldu ve worldRoot sabitlendi.");
    }
}
