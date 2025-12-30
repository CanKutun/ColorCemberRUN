using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class karakter_kontrol : MonoBehaviour
{

    [Header("Efektler")]
    public GameObject yildizPatlama;

    //  YENİ: Ayak altındaki yıldız ParticleSystem (child)
    [Header("Yıldız Efekti (Ayak Altı)")]
    public ParticleSystem yildizFX;

    [Header("Araba Üstüne Düşme Sesi")]
    public AudioClip boingClip;
    private AudioSource boingSource;


    public AudioSource sfxSource;
    public AudioClip coinSesi;

    Rigidbody rigi;

    Collider kendiCollider;

    float ziplama_gucu = 5.0f;
    float kosma_hizi = 2.0f;

    bool sag;
    bool sol;
    bool zipladi = false;
    bool havadaZipladi = false;   // Sadece zıplamayla havaya çıktı mı?

    public ParticleSystem toz;

    Animator anim;

    Transform yol_1;
    Transform yol_2;

    yonetici yonet;



    public GameObject oyun_bitti_paneli;

    public bool miknatis_alindi = false;

    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        kendiCollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();

        yol_1 = GameObject.Find("yol_1").transform;
        yol_2 = GameObject.Find("yol_2").transform;

        yonet = GameObject.Find("yonetici").GetComponent<yonetici>();

        // Boing ses kaynağı oluştur
        boingSource = gameObject.AddComponent<AudioSource>();
        boingSource.playOnAwake = false;
        boingSource.loop = false;
        boingSource.volume = 0.8f;

    }

    //  YENİ: Yıldız efektini güvenli şekilde oynatan yardımcı fonksiyon
    void OynatYildizEfekti()
    {
        if (yildizFX == null)
            return;

        // Önce eski partikülleri temizle, sonra patlat
        yildizFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        yildizFX.Play();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "yol_1")
        {
            yol_2.position = new Vector3(yol_2.position.x, yol_2.position.y, yol_1.position.z + 10.0f);
        }
        if (other.gameObject.name == "yol_2")
        {
            yol_1.position = new Vector3(yol_1.position.x, yol_1.position.y, yol_2.position.z + 10.0f);
        }


        if (other.gameObject.tag == "altin")
        {
            other.gameObject.SetActive(false);
            yonet.puan_arttir(10);

            // SES
            if (sfxSource && coinSesi)
                sfxSource.PlayOneShot(coinSesi, 1f);
        }

        if (other.gameObject.tag == "miknatis")
        {
            other.gameObject.SetActive(false);

            miknatis_alindi = true;

            // SES
            if (CoinSfxPlayer.Instance != null)
                CoinSfxPlayer.Instance.PlayMiknatis();

            Invoke("miknatis_iptal", 10.0f);
        }

    }

    void miknatis_iptal()
    {
        miknatis_alindi = false;
    }




    private void OnCollisionEnter(Collision collision)
    {
        // 1) ÖNCE ARABAYI KONTROL EDELİM
        if (collision.gameObject.CompareTag("Araba"))
        {
            // Tüm temas noktalarına bakalım
            bool usttenCarpti = false;

            foreach (var cp in collision.contacts)
            {
                // Normal yukarı doğru bakıyorsa (üstten temas)
                if (cp.normal.y > 0.5f)
                {
                    usttenCarpti = true;
                    break;
                }
            }

            // Ek güvence: Çocuk arabadan belirgin şekilde yukarıdaysa
            // (kaput / bagaj üstüne düşme durumlarını da "üstten" sayalım)
            if (!usttenCarpti)
            {
                float yukseklikFarki = transform.position.y - collision.transform.position.y;

                // Bu değeri oyuna göre ince ayar yapabilirsin (0.2f - 0.5f arası deneyebilirsin)
                if (yukseklikFarki > 0.3f)
                {
                    usttenCarpti = true;
                }
            }

            if (!usttenCarpti)
            {
                // YANDAN / ÖNDEN / ARKADAN ÇARPTI → ÖL
                oyun_bitti_paneli.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {
                // ÜSTÜNE / ÜST SEVİYEDEN DÜŞTÜ → ÖLDÜRME

                // BOING SESİ
                if (boingClip != null)
                    boingSource.PlayOneShot(boingClip);

                // KÜÇÜK SEKME
                rigi.velocity = Vector3.zero;
                rigi.velocity = Vector3.up * 3f;

                // ⭐ MEVCUT: Prefab yıldız patlama
                if (yildizPatlama != null)
                {
                    Instantiate(
                        yildizPatlama,
                        transform.position + Vector3.up * 0.5f,
                        Quaternion.identity
                    );
                }

                // ⭐ Ayak altı yıldız efekti
                OynatYildizEfekti();

                // ✨ BURAYA EKLENECEK → Araba ile çarpışmayı geçici kapat
                StartCoroutine(GeciciArabaylaCarpismaKapat(collision.collider));
            }

            return; // Araba için işi bitirdik
        }


        // 2) DİĞER ENGELLER (TAŞ / KÜTÜK) → HER TEMAS ÖLÜM
        if (collision.gameObject.CompareTag("engel"))
        {
            oyun_bitti_paneli.SetActive(true);
            Time.timeScale = 0.0f;

            // İstersen buraya da yıldız koyabilirsin:
            // OynatYildizEfekti();
            return;
        }

        // 3) ARABA VE ENGEL HARİCİ ZEMİNLERE ÜSTTEN DÜŞÜNCE YILDIZ ÇIKSIN
        if (collision.contacts.Length > 0)
        {
            ContactPoint cp = collision.contacts[0];

            // Yukarıdan iniş mi? (normal yukarı bakıyorsa)
            // VE bu iniş, zıplama sonrası mı?
            if (cp.normal.y > 0.5f && havadaZipladi)
            {
                OynatYildizEfekti();
                havadaZipladi = false;   // İndi, zıplama bitti
            }
        }
    }



    private void OnCollisionStay(Collision collision)
    {
        zipladi = false;

        if (!toz.isPlaying)
        {
            toz.Play();
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        zipladi = true;

        if (toz.isPlaying)
        {
            toz.Stop();
        }
    }


    // -0.5f   -3.0f


    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch parmak = Input.GetTouch(0);


            if (parmak.deltaPosition.x > 50.0f)
            {
                sag = true;
                sol = false;
            }


            if (parmak.deltaPosition.x < -50.0f)
            {
                sag = false;
                sol = true;
            }

        }




        if (sag == true)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(-0.5f, transform.position.y, transform.position.z), kosma_hizi * Time.deltaTime);
        }


        if (sol == true)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(-3.0f, transform.position.y, transform.position.z), kosma_hizi * Time.deltaTime);

        }



        transform.Translate(0f, 0f, kosma_hizi * Time.deltaTime);
    }


    public void zipla()
    {
        if (zipladi == false)
        {
            anim.SetTrigger("zipla");
            rigi.velocity = Vector3.zero;
            rigi.velocity = Vector3.up * ziplama_gucu;

            //  YENİ: Zıpladı, şimdi havada say
            havadaZipladi = true;

            // SES  Zıplama whoosh
            if (CoinSfxPlayer.Instance != null)
                CoinSfxPlayer.Instance.PlayJump();
        }
    }

    IEnumerator GeciciArabaylaCarpismaKapat(Collider arabaCollider)
    {
        if (kendiCollider == null || arabaCollider == null)
            yield break;

        // Çarpışmayı kapat
        Physics.IgnoreCollision(kendiCollider, arabaCollider, true);

        // Kısa süre bekle
        yield return new WaitForSeconds(0.35f);

        // Çarpışmayı tekrar aç
        Physics.IgnoreCollision(kendiCollider, arabaCollider, false);
    }
}
