using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class yonetici : MonoBehaviour
{


    public TextMeshProUGUI puan_txt;

    public GameObject altin;
    public GameObject miknatis;

    public GameObject kutuk;
    public GameObject tas;
    public GameObject araba;

    List<GameObject> altinlar;
    List<GameObject> digerleri;

    Transform cocuk;

    int puan = 0;

    public GameObject oyunu_durdur_pnl;


    void Start()
    {
        altinlar = new List<GameObject>();
        digerleri = new List<GameObject>();
        cocuk = GameObject.Find("cocuk").transform;

        uretme(altin, 10, altinlar);

        uretme(miknatis, 3, digerleri);
        uretme(kutuk, 3, digerleri);
        uretme(tas, 3, digerleri);
        uretme(araba, 3, digerleri);

        InvokeRepeating("altin_uret", 0.0f, 1.0f);
        InvokeRepeating("engel_uret", 1.5f, 3.0f);

        puan_txt.text = "SKOR " + puan.ToString();
    }


    public void puan_arttir(int deger)
    {
        puan += deger;
        puan_txt.text = "SKOR " + puan.ToString();
    }

    void engel_uret()
    {
        int rast = Random.Range(0, digerleri.Count);

        if (digerleri[rast].activeSelf == false)
        {
            digerleri[rast].SetActive(true);

            //  Eğer bu nesne arabaysa, üzerindeki ArabaKorna'ya cocuk'u ata
            ArabaKorna korna = digerleri[rast].GetComponent<ArabaKorna>();
            if (korna != null)
            {
                korna.cocuk = cocuk;
            }

            int rastgele = Random.Range(0, 2);

            if (rastgele == 0)
            {
                digerleri[rast].transform.position = new Vector3(-0.5f, -3.0f, cocuk.position.z + 10.0f);
            }

            if (rastgele == 1)
            {
                digerleri[rast].transform.position = new Vector3(-3.0f, -3.0f, cocuk.position.z + 10.0f);
            }

            if (digerleri[rast].tag == "miknatis")
            {
                if (cocuk.gameObject.GetComponent<karakter_kontrol>().miknatis_alindi == true)
                {
                    digerleri[rast].SetActive(false);
                }
            }

            if (digerleri[rast].name != "miknatis(Clone)")
            {
                Vector3 eskiPos = digerleri[rast].transform.position;

                eskiPos.y = -3.6f;

                digerleri[rast].transform.position = eskiPos;
            }

        }
        else
        {
            foreach (GameObject nesne in digerleri)
            {
                if (nesne.activeSelf == false)
                {
                    nesne.SetActive(true);

                    //  Eğer bu nesne arabaysa, üzerindeki ArabaKorna'ya cocuk'u ata
                    ArabaKorna korna = nesne.GetComponent<ArabaKorna>();
                    if (korna != null)
                    {
                        korna.cocuk = cocuk;
                    }

                    int rastgele_2 = Random.Range(0, 2);

                    if (rastgele_2 == 0)
                    {
                        nesne.transform.position = new Vector3(-0.5f, -3.0f, cocuk.position.z + 10.0f);
                    }

                    if (rastgele_2 == 1)
                    {
                        nesne.transform.position = new Vector3(-3.0f, -3.0f, cocuk.position.z + 10.0f);
                    }

                    if (nesne.tag == "miknatis")
                    {
                        if (cocuk.gameObject.GetComponent<karakter_kontrol>().miknatis_alindi == true)
                        {
                            nesne.SetActive(false);
                        }
                    }

                    if (digerleri[rast].name != "miknatis(Clone)")
                    {
                        Vector3 eskiPos = digerleri[rast].transform.position;

                        eskiPos.y = -3.6f;

                        digerleri[rast].transform.position = eskiPos;
                    }

                    return;
                }
            }
        }
    }

    public void tekrar_oyna()
    {
        SceneManager.LoadScene("Scenes/oyun");
        Time.timeScale = 1.0f;
    }

    public void devam_et()
    {
        oyunu_durdur_pnl.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void oyunu_durdur()
    {
        oyunu_durdur_pnl.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void oyundan_cik()
    {
        Application.Quit();
    }

    void altin_uret()
    {
        foreach (GameObject altin in altinlar)
        {
            if (altin.activeSelf == false)
            {
                altin.SetActive(true);

                int rastgele = Random.Range(0, 2);

                if (rastgele == 0)
                {
                    altin.transform.position = new Vector3(-0.5f, -3.0f, cocuk.position.z + 10.0f);
                }

                if (rastgele == 1)
                {
                    altin.transform.position = new Vector3(-3.0f, -3.0f, cocuk.position.z + 10.0f);
                }

                return;
            }
        }
    }

    void uretme(GameObject nesne, int miktar, List<GameObject> liste)
    {
        for (int i = 0; i < miktar; i++)
        {
            GameObject yeni_nesne = Instantiate(nesne);
            yeni_nesne.SetActive(false);
            liste.Add(yeni_nesne);
        }
    }
}