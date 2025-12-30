using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArabaFarKontrol : MonoBehaviour
{
    [Header("Far Sprite'ları")]
    public SpriteRenderer[] farSprites;   // FarGlow_L, FarGlow_R buraya gelecek

    [Header("Selektör Renkleri")]
    public Color offColor = new Color(1f, 1f, 1f, 0.25f);  // Sönük
    public Color onColor = new Color(1f, 1f, 1f, 1f);      // Parlak

    [Header("Selektör Hızı (sn)")]
    public float blinkInterval = 0.25f;

    private void Start()
    {
        // Başta farları kapalı göster
        SetColor(offColor);
        StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            // Aç
            SetColor(onColor);
            yield return new WaitForSeconds(blinkInterval);

            // Kapat
            SetColor(offColor);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void SetColor(Color c)
    {
        if (farSprites == null) return;

        foreach (var sr in farSprites)
        {
            if (sr != null)
                sr.color = c;
        }
    }
}