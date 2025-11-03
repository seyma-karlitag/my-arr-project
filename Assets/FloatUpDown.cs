using UnityEngine;

public class FloatUpDown : MonoBehaviour
{
    [Tooltip("Tepe ile dip arasındaki toplam mesafe / 2")]
    public float amplitude = 0.1f;      // yarı salınım (m)
    public float speed = 1.2f;          // saniyedeki tur sayısı (Hz)
    public float phaseOffset = 0f;      // aynı anda başlamasın diye derece cinsinden
    public bool useLocal = true;        // AnchorRoot'a göre (local) hareket
    private bool shouldMove = false;
    Vector3 basePos;

    void Awake()
    {
        basePos = useLocal ? transform.localPosition : transform.position;
    }
    public void setMove()
    {
        var newState = !shouldMove;
        Debug.Log($"FloatUpDown {(newState ? "started" : "stopped")}");
        shouldMove = !shouldMove;
    }
    void Update()
    {
        if (!shouldMove) return;
        float t = Time.time; // istersen Time.timeSinceLevelLoad da olur
        // derece -> radyan
        float phase = phaseOffset * Mathf.Deg2Rad;
        float y = Mathf.Sin((t * speed * 2f * Mathf.PI) + phase) * amplitude;

        if (useLocal)
            transform.localPosition = new Vector3(basePos.x, basePos.y + y, basePos.z);
        else
            transform.position = new Vector3(basePos.x, basePos.y + y, basePos.z);
    }
}
