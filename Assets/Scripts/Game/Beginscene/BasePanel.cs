using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel<T> : MonoBehaviour where T:class
{
    private static T instance;
    public static T Instance => instance;
    protected virtual void Awake()
    {
        instance = this as T;
     
    }
    public virtual void Showme() 
    { this .gameObject.SetActive(true) ;  }
    public virtual void Hideme()
    { this .gameObject.SetActive(false); }

}
