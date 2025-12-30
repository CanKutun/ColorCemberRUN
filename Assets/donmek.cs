using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class donmek : MonoBehaviour
{

    string isim;


    float deger = 500.0f;


    void Start()
    {
        isim = gameObject.tag;    
    }

    
    void Update()
    {
        if (isim == "miknatis")
        {
            transform.Rotate(deger*Time.deltaTime, 0f, 0f);
        }

        if (isim == "altin")
        {
            transform.Rotate(0f,deger*Time.deltaTime, 0f, Space.World);
        }

    }
}
