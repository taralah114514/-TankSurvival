using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKMusic : MonoBehaviour
{   private static BKMusic instance;
    public static BKMusic Instance => instance;
    private AudioSource audiosource;

    // Start is called before the first frame update
    void Awake()
    {
        
        instance = this;
        audiosource = this.GetComponent<AudioSource>();
        ChangeValue(DataManager.Instance.Musicdata.musicValue);
        Openornot(!(DataManager.Instance.Musicdata.isopenMusic));
    }

    // Update is called once per frame
    public void ChangeValue(float value)
    {
        audiosource.volume = value;
    }
    public void Openornot(bool isOpen) {
        audiosource.mute= isOpen;

    }
}
