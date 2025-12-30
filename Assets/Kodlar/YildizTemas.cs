using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YildizTemas : MonoBehaviour
{
    public ParticleSystem starFX;

    private void OnTriggerEnter(Collider other)
    {
        // SADECE coin'leri görmezden gel.
        // Coin tag'in ne ise ona göre burayý ayarla:
        if (other.CompareTag("altin") || other.CompareTag("altin"))
        {
            return;
        }

        Debug.Log("YildizTemas TRIGGER: " + other.name + "  Tag:" + other.tag);

        if (starFX == null)
            return;

        // Her tetiklenmede önce temizle, sonra patlat
        starFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        starFX.Play();
    }
}