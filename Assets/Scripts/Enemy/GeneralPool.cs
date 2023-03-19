using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneralPool : MonoBehaviour
{
    private static GeneralPool instance;
    public static GeneralPool Instance { get { return instance; } }
    public GameObject poolThraaxian;
    public GameObject poolZorgonian;

    private void Awake()
    {
        if(instance == null)
        { 
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this.gameObject);   
        poolThraaxian = transform.GetChild(0).gameObject;
        poolZorgonian = transform.GetChild(1).gameObject;
    }
}
