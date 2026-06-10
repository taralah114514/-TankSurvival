using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObj : MonoBehaviour
{
    public GameObject eff;
    public GameObject[] PropRewardobj;
    private void OnTriggerEnter(Collider other)
    {
        BulletObj bullet = other.GetComponent<BulletObj>();
        if (bullet != null && bullet.shootfather is PlayerObject)
        {
            Hit();
        }
    }

    public void Hit()  // 新增公开方法
    {
        int indexA = Random.Range(0, 3);
        if (indexA == 0)
        {
            indexA = Random.Range(0, PropRewardobj.Length);
            Instantiate(PropRewardobj[indexA], transform.position, transform.rotation);
        }
        GameObject eff = Instantiate(this.eff, transform.position, transform.rotation);
        AudioSource audioSource = eff.GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
        audioSource.volume = DataManager.Instance.Musicdata.soundValue;
        Destroy(gameObject);
    }
}
