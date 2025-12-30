using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class ArabaKorna : MonoBehaviour
{
    public Transform cocuk;
    public float tetiklemeMesafesi = 4f;   // Z mesafesi
    public float seritTolerance = 0.5f;     // X mesafe toleransı
    public bool sadeceBirKez = true;

    AudioSource src;
    bool caldi = false;

    void Start()
    {
        src = GetComponent<AudioSource>();

        if (cocuk == null)
            cocuk = GameObject.FindWithTag("cocuk").transform;
    }

    void Update()
    {
        if (sadeceBirKez && caldi) return;

        // 1) ŞERİT KONTROLÜ (X yakın mı?)
        float xFark = Mathf.Abs(transform.position.x - cocuk.position.x);
        bool ayniSerit = xFark < seritTolerance;

        if (!ayniSerit)
            return; // farklı şerit → korna yok

        // 2) MESAFE KONTROLÜ (Z)
        float zFark = transform.position.z - cocuk.position.z;

        if (zFark < tetiklemeMesafesi && zFark > 0)
        {
            src.Play();
            caldi = true;
        }
    }
}