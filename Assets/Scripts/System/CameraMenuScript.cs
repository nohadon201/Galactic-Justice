using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMenuScript : MonoBehaviour
{
    private Vector3 Rotation;
    void Start()
    {
        StartCoroutine(RotationRandom());
    }

    void Update()
    {
        transform.Rotate(Rotation);
    }
    public IEnumerator RotationRandom()
    {
        
        while (true) {
            Rotation = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
            yield return new WaitForSeconds(3f);  
        }
    }
}
