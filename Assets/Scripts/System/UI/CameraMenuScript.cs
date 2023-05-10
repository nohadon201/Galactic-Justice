using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class CameraMenuScript : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 nextRotation;
    const float rotationRange = 1f;
    float nextRotationTime = 0f;
    float deltaTime = 0f;
    void Start()
    {
        StartCoroutine(RotationRandom());
    }

    void Update()
    {
        deltaTime += Time.deltaTime;
        transform.Rotate(Vector3.Lerp(currentRotation, nextRotation, deltaTime/nextRotationTime) * Time.deltaTime);
    }
    public IEnumerator RotationRandom()
    {

        while (true)
        {
            currentRotation = nextRotation;
            nextRotation = new Vector3(Random.Range(-rotationRange, rotationRange), Random.Range(-rotationRange, rotationRange), Random.Range(-rotationRange, rotationRange));
            nextRotation.Normalize();
            nextRotationTime = Random.Range(3f, 10f);
            deltaTime = 0f;
            yield return new WaitForSeconds(nextRotationTime);
        }
    }
}
