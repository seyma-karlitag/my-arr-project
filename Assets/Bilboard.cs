using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum Mode { Full, YAxisOnly }

    [Header("Target (boşsa otomatik Camera.main)")]
    public Transform target;

    [Header("Dönüş Modu")]
    public Mode mode = Mode.YAxisOnly;     // AR için önerilen

    [Header("Dengeleme")]
    [Tooltip("0 = anlık, 5-10 = yumuşak takip")]
    public float smooth = 8f;

    [Tooltip("Dünyanın yukarı ekseni")]
    public Vector3 worldUp = Vector3.up;

    void LateUpdate()
    {
        if (!target)
        {
            var cam = Camera.main;
            if (!cam) return;
            target = cam.transform;
        }

        Vector3 toCam = transform.position - target.position; // quad'ın yüzü +Z kabul
        if (mode == Mode.YAxisOnly)
        {
            toCam.y = 0f;                       // sadece yatayda dön
            if (toCam.sqrMagnitude < 1e-6f) return;
        }
        toCam.Normalize();

        // İstenen rotasyon
        Quaternion desired = Quaternion.LookRotation(toCam, worldUp);

        // Yumuşatma
        if (smooth <= 0f)
            transform.rotation = desired;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }
}
