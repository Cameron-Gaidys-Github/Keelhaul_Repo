using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Pos : MonoBehaviour
{
    public Transform cam;
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
