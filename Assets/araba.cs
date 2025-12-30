using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class araba : MonoBehaviour
{

    public float deger=2f;


    

    void Update()
    {
        transform.Translate(0f, 0f, deger * Time.deltaTime);
    }
}
