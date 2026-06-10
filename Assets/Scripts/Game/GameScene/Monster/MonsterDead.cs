using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDead : MonoBehaviour
{
    [Header("死亡特效")]
    public GameObject deadEffect;        // 死亡烟雾特效
    [Header("死亡音效")]
    public AudioClip deadSound;
    [Range(0f, 3f)]
    public float volumeScale = 0.5f;     // 音量缩放，资源太大就调小

    void Start()
    {
        // 生成烟雾特效
        if (deadEffect != null)
        {
            GameObject eff = Instantiate(deadEffect, transform.position, deadEffect.transform.rotation);
        }

        // 播放音效，用2D方式不受距离影响
        if (deadSound != null)
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = deadSound;
            audio.spatialBlend = 0.3f;  // 0=2D，1=3D，设0就不受距离衰减
            audio.volume = volumeScale * DataManager.Instance.Musicdata.soundValue;
            audio.mute = !DataManager.Instance.Musicdata.isopenSound;
            audio.Play();
        }

       
    }
}