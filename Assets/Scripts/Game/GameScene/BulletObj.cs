using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    public float movespeed = 50;
    public TankBase shootfather;
    public GameObject effobj;
    public float effectOffset = 0f;  // 正前负后

    private bool isDestroyed = false;

    void Update()
    {
        float moveDistance = movespeed * Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, moveDistance))
        {
            // 忽略自身
            if (hit.collider.gameObject == this.gameObject)
            {
                this.transform.Translate(Vector3.forward * moveDistance);
                return;
            }

            if (isDestroyed) return;
            isDestroyed = true;

            // 生成特效
            if (effobj != null)
            {
              //Quaternion rot = Quaternion.LookRotation(this.transform.forward) * Quaternion.Euler(-90, 0, 0);
                GameObject eff = Instantiate(effobj, hit.point + transform.forward * effectOffset,transform.rotation);
                AudioSource audioSource = eff.GetComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
                audioSource.volume = DataManager.Instance.Musicdata.soundValue;
            }

            // 检测标签，调用受伤逻辑
            if (hit.collider.CompareTag("Cube"))
            {
                CubeObj cube = hit.collider.GetComponent<CubeObj>();
                 if (cube != null) 
                    cube.Hit();
            }
            if (hit.collider.CompareTag("Enemy"))
            {
                Monster monster = hit.collider.GetComponentInParent<Monster>();
                if (monster != null)
                    monster.Wound(shootfather);
            }
            if (hit.collider.CompareTag("Player"))
            {
                PlayerObject player = hit.collider.GetComponentInParent<PlayerObject>();
                if (player != null)
                    player.Wound(shootfather);
            }
            Destroy(this.gameObject);
            return;
        }

        this.transform.Translate(Vector3.forward * moveDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return; 
        
        if (other.gameObject.CompareTag("Cube") )
        {
            isDestroyed = true;
            if (effobj != null)
            {
                Quaternion rot = Quaternion.LookRotation(this.transform.forward) * Quaternion.Euler(-90, 0, 0);
                GameObject eff = Instantiate(effobj, this.transform.position, rot);
               // Quaternion rot = Quaternion.LookRotation(-this.transform.forward);
               // GameObject eff = Instantiate(effobj, this.transform.position, rot);
                // GameObject eff = Instantiate(effobj, this.transform.position, this.transform.rotation);
                // GameObject eff = Instantiate(effobj, this.transform.position, Quaternion.LookRotation(-this.transform.forward));
            }
            Destroy(this.gameObject);
            AudioSource  audioSource=effobj.GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.mute=!DataManager.Instance.Musicdata.isopenSound;
            audioSource.volume=DataManager.Instance.Musicdata.soundValue;


        }
       
    }


    public void SetFather(TankBase obj)
    {
        shootfather = obj;
    }

}