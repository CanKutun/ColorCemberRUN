using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class RunLoopByTag : MonoBehaviour
{
    [Header("Tag Ayarları")]
    public string playerTag = "cocuk";
    public string groundTag = "yol";

    [Header("Ses")]
    public AudioClip runLoopClip;
    [Range(0f, 1f)] public float maxVolume = 0.8f;
    [Range(0.5f, 2f)] public float pitch = 1f;
    public float fadeSpeed = 8f;

    [Header("3D")]
    public bool use3D = true;
    public float min3D = 3f, max3D = 14f;
    public Transform footPoint;

    [Header("Hareket Kontrolü")]
    [Tooltip("Sesin çalması için minimum hareket hızı (unit/sn).")]
    public float minSpeedForSound = 0.05f;
    public float speedSmooth = 8f;

    [Header("Panel/TimeScale ile Durdur")]
    [Tooltip("Time.timeScale = 0 ise ses kapansın mı?")]
    public bool stopWhenTimeScaleZero = true;

    [Tooltip("Bu panellerden biri aktifse ses kapansın.")]
    public GameObject[] stopPanels;      // oyun_bitti_pnl, durdur_pnl vs.

    [Header("Başlangıçta Yerde mi?")]
    public bool raycastInitCheck = true;
    public LayerMask groundMask = ~0;
    public float groundCheckDistance = 0.2f;
    public float footOffsetUp = 0.05f;

    AudioSource src;
    float targetVolume = 0f;
    int groundContacts = 0;
    Vector3 lastPos;
    float smoothedSpeed;

    void Awake()
    {
        if (!string.IsNullOrEmpty(playerTag))
        {
            try { gameObject.tag = playerTag; } catch { }
        }

        if (!footPoint) footPoint = transform;

        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = true;
        src.clip = runLoopClip;
        src.volume = 0f;
        src.pitch = pitch;
        src.spatialBlend = use3D ? 1f : 0f;
        src.minDistance = min3D;
        src.maxDistance = max3D;
        src.dopplerLevel = 0f;

        lastPos = transform.position;
    }

    void Start()
    {
        if (raycastInitCheck && footPoint != null)
        {
            Vector3 origin = footPoint.position + Vector3.up * footOffsetUp;
            if (Physics.Raycast(origin, Vector3.down, out var hit, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != null && hit.collider.CompareTag(groundTag))
                    groundContacts = 1;
            }
        }
    }

    void Update()
    {
        if (!runLoopClip) return;

        // timeScale 0 olsa bile fade düzgün çalışsın
        float dt = Time.timeScale > 0f ? Time.deltaTime : Time.unscaledDeltaTime;
        if (dt <= 0f) dt = 0.0001f;

        // Oyun gerçekten durdu mu? (panel/timeScale)
        bool paused = IsPaused();

        if (paused)
        {
            // Panel veya pause'da: tamamen sustur + STOP (başa sar)
            targetVolume = 0f;
            src.volume = Mathf.MoveTowards(src.volume, targetVolume, fadeSpeed * dt);
            if (src.isPlaying && src.volume <= 0.001f)
                src.Stop();

            lastPos = transform.position;
            return;
        }

        // Hareket hızı hesabı
        Vector3 delta = transform.position - lastPos;
        float rawSpeed = delta.magnitude / dt;
        lastPos = transform.position;

        smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, dt * speedSmooth);

        bool onGround = groundContacts > 0;
        bool movingEnough = smoothedSpeed >= minSpeedForSound;
        bool shouldPlay = onGround && movingEnough;

        // Hedef ses seviyesi
        targetVolume = shouldPlay ? maxVolume : 0f;

        // Volume'ü fade et
        src.volume = Mathf.MoveTowards(src.volume, targetVolume, fadeSpeed * dt);

        // Buradaki fark:
        // - shouldPlay true: Play (devam ettiği yerden)
        // - shouldPlay false: Pause (zıplayınca kaldığı yerden dursun)
        if (shouldPlay)
        {
            if (!src.isPlaying)
                src.Play();   // Pause'dan devam eder, başa sarmaz
        }
        else
        {
            if (src.isPlaying && src.volume <= 0.001f)
                src.Pause();  // zamanı koru, tekrar yere basınca kaldığı yerden
        }

        if (use3D && footPoint != null)
            src.transform.position = footPoint.position;
    }

    bool IsPaused()
    {
        // 1) timeScale kontrolü
        if (stopWhenTimeScaleZero && Time.timeScale == 0f)
            return true;

        // 2) panel kontrolü
        if (stopPanels != null)
        {
            for (int i = 0; i < stopPanels.Length; i++)
            {
                var p = stopPanels[i];
                if (p != null && p.activeInHierarchy)
                    return true;
            }
        }

        return false;
    }

    // ---- Zemin temasları ----
    void OnCollisionEnter(Collision other) { TryEnter(other.collider); }
    void OnCollisionExit(Collision other) { TryExit(other.collider); }
    void OnTriggerEnter(Collider other) { TryEnter(other); }
    void OnTriggerExit(Collider other) { TryExit(other); }

    void TryEnter(Collider col)
    {
        if (col != null && col.CompareTag(groundTag))
            groundContacts++;
    }

    void TryExit(Collider col)
    {
        if (col != null && col.CompareTag(groundTag))
            groundContacts = Mathf.Max(0, groundContacts - 1);
    }
}