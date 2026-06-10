using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankBase : MonoBehaviour
{
    public int atk;
    public int def;
    public int MaxHP;
    public int HP;
    public int reload;

    public GameObject deadEff;

    public float MoveSpeed = 3;
    public float RotateBody=100;
    public float RotateHead=100;

    public Transform TankHead;
    public abstract void Fire();
    public virtual  void Wound(TankBase other)
    {
        int damage = other.atk - def;
        if (damage <= 0)
            return;
        HP -= damage;
        if(HP <= 0)
        {
           HP = 1;
            if (this is PlayerObject)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                win.Instance.Showme(); 
            }
            else Dead();
        }
    }
    public virtual void Dead() 
    {
          
      Destroy(this.gameObject);
      if(deadEff != null) 
      {
            GameObject  effobj =  Instantiate(deadEff,this.transform.position,this.transform.rotation);
            
            AudioSource  audioSource = effobj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = DataManager.Instance.Musicdata.soundValue;
                audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
                audioSource.Play();
            }
        }
    }
}
