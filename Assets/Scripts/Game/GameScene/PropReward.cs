using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_propType 
{
    atk,def,heal,hpadd,reload
}
public class PropReward : MonoBehaviour
{
    public E_propType propType;
    public int atkadd=10;
    public int defadd=3;
    public int MaxHPadd=20;
    public int HPadd=20;
    public int reloadadd=1;
    public GameObject eff;
    AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerObject player = other.GetComponent<PlayerObject>();
            switch (propType )
            {
                case E_propType.atk:
                    player.atk += atkadd; 
                    break;
                case E_propType.def:
                    player.def += defadd;
                    GamePanel.Instance.UpdateDef(player.def);
                    break;
                case E_propType.heal:
                    player.HP += HPadd;
                    if(player.HP>player.MaxHP)player.HP = player.MaxHP;
                    GamePanel.Instance.UpdateHP(player.MaxHP,player.HP);
                    break;
                case E_propType.hpadd:
                    player.HP += MaxHPadd; 
                    player.MaxHP += MaxHPadd;
                    GamePanel.Instance.hpW = ((float)player.MaxHP / (float)100) * 350;
                    GamePanel.Instance.UpdateHP(player.MaxHP, player.HP);
                    break;
                case E_propType.reload:
                    if (player.nowWeapon != null)
                        player.nowWeapon.RefillAmmo();
                    break;
            }
            eff = Instantiate(this.eff, transform.position, transform.rotation);
            audioSource = eff.GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
            audioSource.volume = DataManager.Instance.Musicdata.soundValue;
            Destroy(gameObject);
        }
    }
}