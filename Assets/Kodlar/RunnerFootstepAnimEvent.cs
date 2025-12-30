using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class RunnerFootstepAnimEvent : MonoBehaviour
{
    public ParticleSystem dust;                 // DustController ile aynı partikül
    public CharacterController controller;      // opsiyonel grounded kontrol
    public Transform footPoint;                 // ses konumu (ayağın altı)
    public AudioClip[] dirtStepClips;           // sfx_step_dirt_01..06.wav
    [Range(0f, 1f)] public float volume = 0.85f;
    public Vector2 pitchJitter = new Vector2(0.97f, 1.03f);
    public bool use3D = true;
    public float min3D = 3f, max3D = 14f;
    public float minInterval = 0.10f;           // iki adım arası alt sınır

    AudioSource src;
    float lastTime = -999f;
    ParticleSystem.EmissionModule emission;

    void Awake()
    {
        if (!footPoint) footPoint = transform;
        src = GetComponent<AudioSource>();
        src.playOnAwake = false; src.loop = false;
        src.spatialBlend = use3D ? 1f : 0f;
        src.minDistance = min3D; src.maxDistance = max3D; src.dopplerLevel = 0f;
        if (dust) emission = dust.emission;
    }

    // ANIMATOR EVENT: Koşu klibinde adım anına ekle → Function: OnFootstep
    public void OnFootstep()
    {
        if (Time.time - lastTime < minInterval) return;

        // 1) Yerde mi?
        if (controller && !controller.isGrounded) return;

        // 2) Toz aktif mi? (emission.enabled veya dust.isPlaying)
        bool dustActive = true;
        if (dust) dustActive = emission.enabled && dust.isPlaying;
        if (!dustActive) return;

        // 3) Ses çal
        var clip = Pick(dirtStepClips);
        if (!clip) return;
        src.transform.position = footPoint.position;
        src.pitch = Random.Range(pitchJitter.x, pitchJitter.y);
        src.PlayOneShot(clip, volume);
        lastTime = Time.time;
    }

    AudioClip Pick(AudioClip[] set) => (set == null || set.Length == 0) ? null : set[Random.Range(0, set.Length)];
}