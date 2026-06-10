using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public GameObject bullets;
    public Transform[] shootPosition;
    public TankBase shootfather;

    [Header("闪光")]
    public GameObject muzzleFlashEffect;
    public float muzzleFlashTime = 0.2f;
    public AudioClip[] fireSounds;
    private AudioSource audioSource;

    [Header("后坐力")]
    public Transform recoilPos;
    public float recoilTime = 0.2f;
    public float returnTime = 0.3f;

    [Header("炮口烟")]
    public GameObject PracticeSmoke;
    public float smokeOffset = 0.2f;

    [Header("弹药设置")]
    public int maxAmmo = 5;
    public float reloadTime = 2f;
    public float fireInterval = 0.5f;

    private int currentAmmo;
    private float reloadTimer = 0f;
    private float fireCooldown = 0f;
    private Vector3 originalPos;
    private bool isRecoiling = false;
    private float recoilTimer = 0f;
    private bool canFire = true;
    [Header("射击误差")]
    public float maxSpreadAngle = 15f;  // 最大散布角度，Inspector里调


    public void SetFather(TankBase obj)
    {
        shootfather = obj;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
        audioSource.volume = DataManager.Instance.Musicdata.soundValue;

        originalPos = transform.localPosition;
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        // 冷却倒计时
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        // 后坐力动画
        if (isRecoiling)
        {
            canFire = false;
            recoilTimer += Time.deltaTime;

            if (recoilTimer < recoilTime)
            {
                float t = recoilTimer / recoilTime;
                float sinT = Mathf.Sin(t * Mathf.PI * 0.5f);
                transform.localPosition = Vector3.Lerp(originalPos, recoilPos.localPosition, sinT);
            }
            else if (recoilTimer < recoilTime + returnTime)
            {
                float t = (recoilTimer - recoilTime) / returnTime;
                transform.localPosition = Vector3.Lerp(recoilPos.localPosition, originalPos, t);
            }
            else
            {
                isRecoiling = false;
                transform.localPosition = originalPos;
            }
        }
        else
            canFire = true;

        // 自动装弹（敌人武器无需UI更新）
        if (currentAmmo < maxAmmo)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadTime)
            {
                reloadTimer = 0f;
                currentAmmo++;
            }
        }
    }

    public void Fire()
    {
        Quaternion fireRotation;
        if (!canFire)
            return;
        if (fireCooldown > 0f)
            return;
        if (currentAmmo <= 0)
            return;

        // 生成子弹
        for (int i = 0; i < shootPosition.Length; i++)
        {

            if (Random.value < 0.25f)
            {
                // 50% 精准命中，不加误差
                fireRotation = shootPosition[i].rotation;
            }
            else
            {
                // 50% 只在Y轴（水平方向）随机偏转
                Quaternion spread = Quaternion.Euler(
                    0,  // X轴不动，不上下偏
                    Random.Range(-maxSpreadAngle, maxSpreadAngle),  // 只左右偏
                    0
                );
                fireRotation = shootPosition[i].rotation * spread;
            }

            GameObject obj = Instantiate(bullets, shootPosition[i].position, fireRotation);
            BulletObj bullet = obj.GetComponent<BulletObj>();
            if (bullet != null)
                bullet.SetFather(shootfather);
            if (bullet != null) bullet.SetFather(shootfather);
            // 播放音效
            if (fireSounds != null && fireSounds.Length > 0)
            {
                audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
                audioSource.volume = DataManager.Instance.Musicdata.soundValue;
                int randomIndex = Random.Range(0, fireSounds.Length);
                audioSource.PlayOneShot(fireSounds[randomIndex]);
            }

            // 枪口闪光
            if (muzzleFlashEffect != null)
            {
                GameObject flash = Instantiate(muzzleFlashEffect, shootPosition[i].position, shootPosition[i].rotation);
                Destroy(flash, muzzleFlashTime);
            }

            // 炮口烟
            if (PracticeSmoke != null)
            {
                Vector3 smokePos = shootPosition[i].position - shootPosition[i].forward * smokeOffset;
                Quaternion rot = shootPosition[i].rotation * Quaternion.Euler(0, 0, -180);
                GameObject smoke = Instantiate(PracticeSmoke, smokePos, rot);
                smoke.transform.SetParent(shootPosition[i]);
            }
        }

        currentAmmo--;
        fireCooldown = fireInterval;
        reloadTimer = 0f;

        if (!isRecoiling)
        {
            isRecoiling = true;
            recoilTimer = 0f;
        }
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
}