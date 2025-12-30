using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform cameraTransform;
    public ParticleSystem fireflySag;
    public ParticleSystem fireflySol;

    [Header("Hýz Ayarý")]
    public float minCameraSpeed = 0.01f;
    public float maxCameraSpeed = 0.2f;

    [Header("Emission Aralýðý")]
    public float minEmission = 3f;
    public float maxEmission = 15f;

    private Vector3 lastCamPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCamPos = cameraTransform.position;
    }

    void Update()
    {
        float camSpeed =
            Vector3.Distance(cameraTransform.position, lastCamPos) / Time.deltaTime;

        lastCamPos = cameraTransform.position;

        float t = Mathf.InverseLerp(minCameraSpeed, maxCameraSpeed, camSpeed);
        float emission = Mathf.Lerp(maxEmission, minEmission, t);

        ApplyEmission(fireflySag, emission);
        ApplyEmission(fireflySol, emission);
    }

    void ApplyEmission(ParticleSystem ps, float value)
    {
        if (!ps) return;

        var em = ps.emission;
        em.rateOverTime = value;
    }
}