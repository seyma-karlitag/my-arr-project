using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;     // New Input System
using UnityEngine.EventSystems;    // UI filtresi

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class VideoQuad : MonoBehaviour
{
    [Header("Video Kaynağı")]
    public VideoClip clip;             // Yerel dosya için
    public string url;                 // Streaming için (mp4 vs) – clip boşsa kullanılır
    public bool playOnStart = false;
    public bool loop = true;
    public bool audioEnabled = true;

    [Header("Atamalar")]
    public Camera viewCamera;          // Boşsa Camera.main
    public LayerMask tapMask = ~0;     // İstersen sadece bu objenin layer’ını ver

    VideoPlayer vp;
    AudioSource audioSrc;
    Renderer rend;
    string texProp = "_MainTex";       // ya da "_BaseMap" (URP)

    void Reset()
    {
        // Sahneye eklendiğinde otomatik collider verelim
        if (!TryGetComponent<Collider>(out _))
            gameObject.AddComponent<BoxCollider>();
    }

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (!viewCamera) viewCamera = Camera.main;

        // Mat kaplama özelliği
        texProp = rend.sharedMaterial && rend.sharedMaterial.HasProperty("_BaseMap") ? "_BaseMap" : "_MainTex";

        // VideoPlayer kur
        vp = gameObject.AddComponent<VideoPlayer>();
        vp.playOnAwake = false;
        vp.isLooping = loop;

        if (clip)
        {
            vp.source = VideoSource.VideoClip;
            vp.clip = clip;
        }
        else if (!string.IsNullOrEmpty(url))
        {
            vp.source = VideoSource.Url;
            vp.url = url;
        }

        // Çıkışı materyale yaz (Material Override) – quad’ın materyaline doğrudan
        vp.renderMode = VideoRenderMode.MaterialOverride;
        vp.targetMaterialRenderer = rend;
        vp.targetMaterialProperty = texProp;

        // Ses
        if (audioEnabled)
        {
            audioSrc = gameObject.GetComponent<AudioSource>();
            if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
            audioSrc.playOnAwake = false;
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
            vp.EnableAudioTrack(0, true);
            vp.SetTargetAudioSource(0, audioSrc);
        }
        else
        {
            vp.audioOutputMode = VideoAudioOutputMode.None;
        }

        // Hazırlanınca otomatik başlat istersen
        vp.prepareCompleted += _ => { if (playOnStart) Play(); };
        vp.errorReceived += (_, msg) => Debug.LogError("[VideoQuad] Video error: " + msg);

        vp.Prepare();
    }

    void Update()
    {
        // UI üstünde tıklamayı yok say
        if (!TryGetTap(out var screenPos)) return;

        // Sadece bu quad’a vurulmuş mu?
        if (!viewCamera) viewCamera = Camera.main;
        var ray = viewCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 50f, tapMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider && hit.collider.gameObject == gameObject)
            {
                TogglePlayPause();
            }
        }
    }

    bool TryGetTap(out Vector2 pos)
    {
        pos = default;

        // Mouse (Editor/PC)
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject(-1)) return false;
            pos = Mouse.current.position.ReadValue();
            return true;
        }
#endif
        // Touch (Mobile)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject(0)) return false;
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }
        return false;
    }

    public void TogglePlayPause()
    {
        if (vp == null) return;
        if (!vp.isPrepared)
        {
            vp.Prepare(); // henüz hazır değilse
            return;
        }

        if (!vp.isPlaying)
            Play();
        else
            Pause();
    }

    public void Play()
    {
        if (!vp) return;
        if (!vp.isPrepared) { vp.Prepare(); return; }
        vp.Play();
        if (audioEnabled && audioSrc) audioSrc.Play();
    }

    public void Pause()
    {
        if (!vp) return;
        vp.Pause();
        if (audioEnabled && audioSrc) audioSrc.Pause();
    }
}
