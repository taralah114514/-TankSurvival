using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AutoDestroy : MonoBehaviour
{
    public float delay = 5f;

    void Start()
    {
        Destroy(this.gameObject, delay);
    }
}