using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject aux;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(aux.transform.position);
        Debug.Log(aux.transform.localPosition);
    }
}
