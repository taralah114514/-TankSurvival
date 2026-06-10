using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    public float RotateSpeed = 50;
    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, RotateSpeed*Time.deltaTime);
    }
}
