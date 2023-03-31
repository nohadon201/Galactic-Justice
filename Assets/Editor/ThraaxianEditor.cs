using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Thraaxian))]
public class ThraaxianEditor : Editor
{

    private void OnSceneGUI()
    {
        Thraaxian fov = (Thraaxian)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);
        if (fov)
        {
            Handles.color = Color.blue;
        }
        else
        {
            Handles.color = Color.red;
        }
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.transform.GetChild(1).gameObject.GetComponent<SphereCollider>().radius);


    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
