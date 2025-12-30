using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;
    public float cycleDuration = 60f;

    [Header("Renkler")]
    public Color dayColor = new Color(1f, 0.95f, 0.8f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.25f);

    [Header("Parlaklık")]
    public float dayIntensity = 1.2f;
    public float nightIntensity = 0.2f;

    [Header("Ambient (Ortam Işığı)")]
    public Color dayAmbient = new Color(0.7f, 0.7f, 0.8f);
    public Color nightAmbient = new Color(0.05f, 0.05f, 0.1f);

    [Header("Ekran Üstü Gece Filtresi")]
    public Image nightOverlay;
    public float maxOverlayAlpha = 0.35f;

    [Header("Ateş Böcekleri (ParticleSystem)")]
    public ParticleSystem atesSag;
    public ParticleSystem atesSol;
    [Range(0f, 1f)] public float nightThreshold = 0.6f;

    private float time;
    private bool isNight;

    void Start()
    {
        // Başlangıçta kapalı (gündüz)
        SetFirefly(atesSag, false);
        SetFirefly(atesSol, false);
    }

    void Update()
    {
        if (sun == null) return;

        time += Time.deltaTime;
        float half = cycleDuration / 2f;
        float t = Mathf.PingPong(time / half, 1f); // 0=gündüz, 1=gece

        sun.color = Color.Lerp(dayColor, nightColor, t);
        sun.intensity = Mathf.Lerp(dayIntensity, nightIntensity, t);
        RenderSettings.ambientLight = Color.Lerp(dayAmbient, nightAmbient, t);

        float angle = Mathf.Lerp(50f, -10f, t);
        sun.transform.rotation = Quaternion.Euler(angle, 0f, 0f);

        if (RenderSettings.skybox != null)
        {
            float exposure = Mathf.Lerp(1.1f, 0.3f, t);
            RenderSettings.skybox.SetFloat("_Exposure", exposure);

            Color dayTint = Color.white;
            Color nightTint = new Color(0.12f, 0.18f, 0.35f);
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(dayTint, nightTint, t));
        }

        if (nightOverlay != null)
        {
            float a = Mathf.Lerp(0f, maxOverlayAlpha, t);
            Color c = nightOverlay.color;
            c.a = a;
            nightOverlay.color = c;
        }

        bool nowNight = t >= nightThreshold;
        if (nowNight != isNight)
        {
            isNight = nowNight;
            SetFirefly(atesSag, isNight);
            SetFirefly(atesSol, isNight);
        }
    }

    void SetFirefly(ParticleSystem ps, bool on)
    {
        if (!ps) return;

        if (on)
        {
            if (!ps.isPlaying) ps.Play(true);
        }
        else
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}