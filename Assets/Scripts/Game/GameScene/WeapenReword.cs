using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapenReword : MonoBehaviour
{
    public GameObject[] weapenObj; 
    public GameObject eff; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            int index =Random.Range(0, weapenObj.Length);
            PlayerObject player = other.GetComponent<PlayerObject>();
            player.ChangeWeapon(weapenObj[index]);
            GameObject eff=Instantiate (this.eff,transform.position,transform.rotation);
            AudioSource audioSource = eff.GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
            audioSource.volume = DataManager.Instance.Musicdata.soundValue;
            Destroy(gameObject);
        }
    }
}
