using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(AudioSource))]
public class PanelHitSfx : MonoBehaviour
{
    public AudioClip hitClip;
    public bool randomPitch = true;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // Panel açýldýðý anda bir kere çalsýn
        if (src == null) src = GetComponent<AudioSource>();
        if (src == null || hitClip == null) return;

        if (randomPitch)
        {
            src.pitch = Random.Range(minPitch, maxPitch);
        }

        src.PlayOneShot(hitClip);
    }
}
