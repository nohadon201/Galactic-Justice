using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] private bool initPositive;
    [SerializeField] private TypeMovement type;
    private float level, positionToGo;
    Rigidbody rb;
    public float Diff = 5;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        if (type == TypeMovement.VERTICAL)
        {
            level = transform.position.y;
        }
        else
        {
            level = transform.position.x;
        }
        if (initPositive)
            positionToGo = level + Diff;
        else
            positionToGo = level - Diff;
        StartCoroutine(changePosition());
    }
    void Update()
    {
        if (type == TypeMovement.VERTICAL)
            rb.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, positionToGo, transform.position.z), 0.01f);
        else
            rb.position = Vector3.Lerp(transform.position, new Vector3(positionToGo, transform.position.y, transform.position.z), 0.01f);
    }
    private IEnumerator changePosition()
    {
        while(true)
        {
            yield return new WaitForSeconds(3f);
            positionToGo = positionToGo > level ? level - Diff : level + Diff;
        }
    }
}
public enum TypeMovement
{
    VERTICAL, HORIZONTAL
}