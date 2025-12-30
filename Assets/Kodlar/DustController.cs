using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustController : MonoBehaviour
{
    public ParticleSystem dust;             // ayaktaki toz partikülü
    public CharacterController controller;  // koþucu (veya kendi grounded mantýðýn)
    public Animator animator;               // koþma/jump parametreleri için (opsiyonel)
    [Tooltip("Animator'da koþarken true olan parametre adý (opsiyonel).")]
    public string runBoolName = "IsRunning";

    ParticleSystem.EmissionModule emission;

    void Awake()
    {
        if (!dust) dust = GetComponent<ParticleSystem>();
        emission = dust.emission;

        // Partikülün kontrolü bizde olsun:
        // Play On Awake'i kapatalým, sürekli play yerine emisyonu aç/kapat yapalým.
        if (dust && dust.main.playOnAwake)
        {
            var main = dust.main;
            main.playOnAwake = false;
        }
        dust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        emission.enabled = false;
    }

    void Update()
    {
        bool grounded = controller ? controller.isGrounded : true;
        bool isRunning = true;

        if (animator && !string.IsNullOrEmpty(runBoolName))
            isRunning = animator.GetBool(runBoolName);

        // Koþuyor + yerde ise tozu aç, deðilse kapat
        bool shouldEmit = grounded && isRunning;

        if (shouldEmit && !dust.isPlaying)
        {
            emission.enabled = true;
            dust.Play();
        }
        else if (!shouldEmit && dust.isPlaying)
        {
            emission.enabled = false;
            dust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}