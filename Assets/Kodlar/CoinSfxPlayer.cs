using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSfxPlayer : MonoBehaviour
{
    public static CoinSfxPlayer Instance;

    [Header("Ses Kaynaðý")]
    public AudioSource sfxSource;       // cocuk üstündeki AudioSource

    [Header("Coin Sesleri")]
    public AudioClip[] coinClips;       // coin_chime_1..3

    [Range(0f, 1f)] public float volume = 0.9f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Mýknatýs Sesi")]
    public AudioClip miknatisClip;
    public float miknatisVolume = 1f;

    public void PlayMiknatis()
    {
        if (!sfxSource || !miknatisClip) return;

        sfxSource.pitch = 1f;        // mýknatýs sesi için pitch jitter yok
        sfxSource.PlayOneShot(miknatisClip, miknatisVolume);
    }


    [Header("Zýplama Sesi")]
    public AudioClip jumpWhoosh;
    public float jumpVolume = 0.9f;

    public void PlayJump()
    {
        if (!sfxSource || !jumpWhoosh) return;
        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(jumpWhoosh, jumpVolume);
    }



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!sfxSource)
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }

    public void PlayCoin()
    {
        if (!sfxSource || coinClips == null || coinClips.Length == 0)
            return;

        var clip = coinClips[Random.Range(0, coinClips.Length)];
        if (!clip) return;

        sfxSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        sfxSource.PlayOneShot(clip, volume);
    }
}