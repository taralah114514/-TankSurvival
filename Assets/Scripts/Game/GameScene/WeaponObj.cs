using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObj : MonoBehaviour
{
    public GameObject bullets;
    public Transform[] shootPosition;
    public TankBase shootfather;
    [Header("闪光")]
    public GameObject muzzleFlashEffect;
    public float muzzleFlashTIme=0.2f;
    public AudioClip[] fireSounds;     // 拖入开炮音效
    private AudioSource audioSource;
    //后坐力相关
    [Header("后坐力")]
    public Transform recoilPos;      // 后坐力位置（空物体）
    public float recoilTime = 0.2f;  // 后坐力移动时间
    public float returnTime = 0.3f;  // 返回原位置时间
    [Header("")]
    private Vector3 originalPos;     // 初始位置
    private bool isRecoiling = false;
    private float recoilTimer = 0f;
    private bool canFire = true;  // 能否开火
    [Header("炮口烟")]
    public GameObject PracticeSmoke;
    public float smokeOffset =0.2f;
    [Header("弹药设置")]
    public int maxAmmo = 5;           // 最大弹药数
    public float reloadTime = 2f;     // 每颗子弹填充时间
    public float fireInterval = 0.5f; // 发射后冷却时间
    [HideInInspector]
    public int currentAmmo;          // 当前弹药
    private float reloadTimer = 0f;   // 填充计时器
    [HideInInspector]
    public float fireCooldown = 0f;  // 发射冷却计时器
    [Header("弹药音效")]
    public AudioClip reloadSound;
    public AudioClip cooldownSound;
    private bool cooldownFinished = false;

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
        originalPos = transform.localPosition;  // 记录初始位置
        currentAmmo = maxAmmo;

    }
    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

       
        if (isRecoiling)
        {
            canFire = false;
            recoilTimer += Time.deltaTime;

            if (recoilTimer < recoilTime)
            {
                // 后坐力阶段：sin曲线先快后慢
                float t = recoilTimer / recoilTime;
                float sinT = Mathf.Sin(t * Mathf.PI * 0.5f);  // 0→1，sin曲线
                transform.localPosition = Vector3.Lerp(originalPos, recoilPos.localPosition, sinT);
            }
            else if (recoilTimer < recoilTime + returnTime)
            {
                // 返回阶段：sin曲线先快后慢
                float t = (recoilTimer - recoilTime) / returnTime;
                transform.localPosition = Vector3.Lerp(recoilPos.localPosition, originalPos, t);
            }
            else
            {
                // 结束
                isRecoiling = false;
                transform.localPosition = originalPos;
               
            }
        }
        else
            canFire = true;  // 恢复开火

        if (currentAmmo < maxAmmo)
        {
            reloadTimer += Time.deltaTime;
            GamePanel.Instance.UpdateReload(reloadTimer, reloadTime, true);

            if (reloadTimer >= reloadTime)
            {
                reloadTimer = 0f;
                currentAmmo++;
                GamePanel.Instance.UpdateAmmo(currentAmmo, maxAmmo);

                if (reloadSound != null)
                {
                    audioSource.PlayOneShot(reloadSound);
                }
            }
        }
        else
        {
            // 满弹隐藏
            GamePanel.Instance.UpdateReload(0, reloadTime, false);
        }
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
            GamePanel.Instance.UpdateCooldown(fireCooldown, fireInterval);
            cooldownFinished = false;  // 冷却中重置标志

            if (fireCooldown <= 0f)
            {
                fireCooldown = 0f;
                if (cooldownSound != null && !cooldownFinished)
                {
                    cooldownFinished = true;  // 标记已播放
                    audioSource.PlayOneShot(cooldownSound);
                }
            }
        }
        else
        {
            GamePanel.Instance.UpdateCooldown(0, fireInterval);
        }

    }

    public void Fire()
    {
        if (!canFire) 
        { return; }
        if (fireCooldown > 0f) return;
        if (currentAmmo <= 0) return;
        CannonRecoil recoil = GetComponent<CannonRecoil>();
        if (recoil != null && !recoil.CanFire) return;

        for (int i = 0; i < shootPosition.Length; i++)
        {
            GameObject obj = Instantiate(bullets, shootPosition[i].position, shootPosition[i].rotation);
            BulletObj bullet = obj.GetComponent<BulletObj>(); 
            
            if (bullet != null)
            {
                bullet.SetFather(shootfather);
            }
            if (fireSounds != null)
            {
                audioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
                audioSource.volume = DataManager.Instance.Musicdata.soundValue;
                int randomIndex = Random.Range(0, fireSounds.Length);
                audioSource.PlayOneShot(fireSounds[randomIndex]);
            }

            // 枪口闪光放这里
            if (muzzleFlashEffect != null)
            {
                GameObject flash = Instantiate(muzzleFlashEffect,shootPosition[i].position, shootPosition[i].rotation);

                Destroy(flash, muzzleFlashTIme);
            }
            if (PracticeSmoke != null)
            {
                Vector3 smokePos = shootPosition[i].position - shootPosition[i].forward * smokeOffset;
                Quaternion rot = shootPosition[i].rotation * Quaternion.Euler(0, 0, -180);
                GameObject smoke = Instantiate(PracticeSmoke, smokePos, rot);
                smoke.transform.SetParent(shootPosition[i]);
            }
        }
        currentAmmo--;
        GamePanel.Instance.UpdateAmmo(currentAmmo, maxAmmo);

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
    public bool CanFire() => fireCooldown <= 0f && currentAmmo > 0;
    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        reloadTimer = 0f;
        GamePanel.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }
}
