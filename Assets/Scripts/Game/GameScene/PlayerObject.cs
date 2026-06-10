using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerObject : TankBase
{
    public WeaponObj nowWeapon;
    public Transform WeaponPos;
    public GameObject aimUIPanel;
    //[Header("�����ƽ������")]
    //public Transform normalCamPos;   // ����λ�õ�Transform�������壩
    //public Transform aimCamPos;      // ��׼λ�õ�Transform�������壩

    //public float camSmoothTime = 0.3f;  // ƽ��ʱ��
    //public float normalFOV = 60f;    // ������Ұ
    //public float aimFOV = 30f;       // ��׼��Ұ

    //private Camera mainCam;
    //private bool isAiming = false;
    [Header("������������")]
    public Transform[] camNodes = new Transform[3]; // 3���ڵ㣬��˳���
    public float scrollSensitivity = 0.1f;          // ����������
    public float camSmoothSpeed = 5f;               // ƽ���ٶ�
    [Range(0f, 1f)]
    public float currentT = 0.5f;                   // ��ǰ��·���ϵ�λ�� 0~1

    [Header("��׼����")]
    public Transform aimCamPos;
    public float aimFOV = 30f;
    public float normalFOV = 60f;
    public float fovSmooth = 5f;

    private Camera mainCam;
    public bool isAiming = false;
    private float targetT;
    [Header("��������")]
    public float normalSpeed ;     // �����ٶ�
    public float boostSpeed = 5f;     // �����ٶ�
    public float accelerateTime = 1f;  // ���ٵ������Ҫ��ʱ��
    public float decelerateTime = 0.3f;  // ���ٻ�������Ҫ��ʱ��
    private float currentSpeed;
    private float targetSpeed;
    [Header("������")]
    public float shakeDuration = 0.2f;   // ��ʱ��
    public float shakeMagnitude = 0.1f;  // �𶯷���
    [Header("����������")]
    public float woundShakeDuration = 0.4f;   // ������ʱ�䣬�ȷ������
    public float woundShakeMagnitude = 0.3f;  // �����𶯷��ȣ��ȷ����ǿ

    private Vector3 camOriginalPos;
    private bool isShaking = false;

    [Header("����")]
    public GameObject PracticeSmoke;
    public Transform normalSmokePos;
    public Transform speedupSmokePos;
    public float minSmokeInterval = 5f;  // ��̼��
    public float maxSmokeInterval = 15f; // ����
    private float smokeTimer = 0f;
    private float nextSmokeTime = 0f;
    public float shiftTriggerTime = 2f;   // ��ס��ÿ�ʼ����
    public float boostSmokeInterval = 0.5f; // ���ɼ��
    public int maxBoostSmoke = 2;         // ���ͬʱ��������

    private float shiftHoldTime = 0f;
    private float boostSmokeTimer = 0f;
    [Header("��׼��λ�ж�")]
    public float aimThreshold = 0.1f;  // �������㵽λ��Inspector���
    [Header("̹����Ч����")]
    public AudioClip engineSound;
    public float idleVolumeScale = 0.2f;
    public float normalVolumeScale = 0.5f;
    public float boostVolumeScale = 0.9f;
    public float volumeFadeTime = 1f;
    private AudioSource engineAudioSource;
    private float currentVolume;
    public float a = 0.99f;


    void Start()
    {
       
        mainCam = Camera.main;  // �Զ���ȡ�������
        targetT = currentT;
        currentSpeed = normalSpeed;
        targetSpeed = normalSpeed;
        MoveSpeed = normalSpeed;
        nextSmokeTime = Random.Range(minSmokeInterval, maxSmokeInterval);

        engineAudioSource = gameObject.AddComponent<AudioSource>();
        currentVolume = normalVolumeScale * DataManager.Instance.Musicdata.soundValue;
        engineAudioSource.volume = currentVolume;
        engineAudioSource.loop = true;
        engineAudioSource.clip = engineSound;
        engineAudioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
        engineAudioSource.Play();

    }
    // Update is called once per frame
    void Update()
    {
        UpdateEngineSound();
        smokeTimer += Time.deltaTime;
        Quaternion rot = normalSmokePos.rotation * Quaternion.Euler(0, 0, -180);
        // ���ֿ���·��λ��
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetT = Mathf.Clamp01(targetT - scroll * scrollSensitivity);
        currentT = Mathf.Lerp(currentT, targetT, camSmoothSpeed * Time.deltaTime);
       
        // �Ҽ���׼
        if (Input.GetMouseButtonDown(1)) { isAiming = true; aimUIPanel.SetActive(true); }
        if (Input.GetMouseButtonUp(1)) {isAiming = false; aimUIPanel.SetActive(false); }

        if (!isAiming)
        {
            // ����currentT��·���ϲ�ֵ
            Vector3 targetPos = GetPathPosition(currentT);
            Quaternion targetRot = GetPathRotation(currentT);
            if (!isShaking)  // ��������
            {
                mainCam.transform.position = Vector3.Lerp(
                    mainCam.transform.position, targetPos, camSmoothSpeed * Time.deltaTime);
            }
            mainCam.transform.rotation = Quaternion.Slerp(
                mainCam.transform.rotation, targetRot, camSmoothSpeed * Time.deltaTime);

        

            mainCam.fieldOfView = Mathf.Lerp(
                mainCam.fieldOfView, normalFOV, fovSmooth * Time.deltaTime);
        }
        else
        {
            // ��׼
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, aimCamPos.position, camSmoothSpeed * Time.deltaTime);
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, aimCamPos.rotation, camSmoothSpeed * Time.deltaTime);
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, aimFOV, fovSmooth * Time.deltaTime);
        }
        // Shift���� �� ��������
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        { targetSpeed = boostSpeed;
        shiftHoldTime += Time.deltaTime;

        if (shiftHoldTime >= shiftTriggerTime)
        {
            boostSmokeTimer += Time.deltaTime;
            if (boostSmokeTimer >= boostSmokeInterval)
            {
                boostSmokeTimer = 0f;
                if (speedupSmokePos.childCount < maxBoostSmoke && PracticeSmoke != null)
                {
                        GameObject smoke = Instantiate(PracticeSmoke, speedupSmokePos.position, rot);
                        smoke.transform.SetParent(speedupSmokePos);
                }
            }
        }
        }
        else
        {
            targetSpeed = normalSpeed;
            shiftHoldTime = 0f;      // ������
            boostSmokeTimer = 0f;    // ������
        }
        // ƽ������
        if (currentSpeed < targetSpeed)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed,(boostSpeed - normalSpeed) / accelerateTime * Time.deltaTime);
        else
          currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed,(boostSpeed - normalSpeed) / decelerateTime * Time.deltaTime);
        MoveSpeed = currentSpeed;

        //float vertical = Input.GetAxis("Vertical");
        //float horizontal = Input.GetAxis("Horizontal");
      
            
        ////float vertical = Input.GetAxis("Vertical");
        ////float horizontal = Input.GetAxis("Horizontal");

        

        //// ǰ���ƶ�����
        //if (Mathf.Abs(vertical) > a)
        //{
        //    transform.Translate(vertical * Vector3.forward * currentSpeed * Time.deltaTime);
        //}

        //// ������ת����
        //if (Mathf.Abs(horizontal) > a)
        //{
        //    transform.Rotate(horizontal * Vector3.up * RotateBody * Time.deltaTime);
        //}

        float vertical = 0f;
        float horizontal = 0f;

        // WS
        if (Input.GetKey(KeyCode.W))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S))
            vertical = -1f;

        // AD
        if (Input.GetKey(KeyCode.D))
            horizontal = 1f;
        else if (Input.GetKey(KeyCode.A))
            horizontal = -1f;

        // �ƶ�
        transform.Translate(vertical * Vector3.forward * currentSpeed * Time.deltaTime);

        // ת��
        if (vertical != 0)
        {
            transform.Rotate(horizontal * Vector3.up * RotateBody * Time.deltaTime);
        }

        //��׼���
        TankHead.transform.Rotate(Input.GetAxis("Mouse X") * Vector3.up * RotateHead * Time.deltaTime);
        if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1) &&
    isAiming && IsAimReady() && nowWeapon != null && nowWeapon.CanFire())
        {
            nowWeapon.SetFather(this);
            Fire();
            isAiming = false;
            aimUIPanel.SetActive(false);
        }
        //smoke

        if (smokeTimer >= nextSmokeTime)
        {
            smokeTimer = 0f; 
            nextSmokeTime = Random.Range(minSmokeInterval, maxSmokeInterval);

            // ����������
            if (PracticeSmoke != null && normalSmokePos != null)
            {
                GameObject smoke = Instantiate(PracticeSmoke, normalSmokePos.position, rot);
                smoke.transform.SetParent(normalSmokePos);
            }
        }
    }

    private void UpdateEngineSound()
    {
        bool isMoving = Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f;
        bool shouldBoost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        float scale = shouldBoost ? boostVolumeScale : isMoving ? normalVolumeScale : idleVolumeScale;
        float targetVolume = scale * DataManager.Instance.Musicdata.soundValue;

        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume,(boostVolumeScale * DataManager.Instance.Musicdata.soundValue) / volumeFadeTime * Time.deltaTime);

        engineAudioSource.volume = currentVolume;
        engineAudioSource.mute = !DataManager.Instance.Musicdata.isopenSound;
    }
    private bool IsAimReady()
    {
        // �ж������λ�ú���ת�Ƿ��Ѿ��ӽ���׼λ��
        float posDist = Vector3.Distance(mainCam.transform.position, aimCamPos.position);
        float rotDist = Quaternion.Angle(mainCam.transform.rotation, aimCamPos.rotation);

        return posDist < aimThreshold && rotDist < 1f;
    }
    private Vector3 GetPathPosition(float t)
    {
        // ����·�����ڵ�0��1���ڵ�1��2
        float scaledT = t * (camNodes.Length - 1);
        int index = Mathf.Clamp((int)scaledT, 0, camNodes.Length - 2);
        float localT = scaledT - index;

        return Vector3.Lerp(camNodes[index].position, camNodes[index + 1].position, localT);
    }

    // ����tֵ��ȡ·���ϵ���ת
    private Quaternion GetPathRotation(float t)
    {
        float scaledT = t * (camNodes.Length - 1);
        int index = Mathf.Clamp((int)scaledT, 0, camNodes.Length - 2);
        float localT = scaledT - index;

        return Quaternion.Slerp(camNodes[index].rotation, camNodes[index + 1].rotation, localT);
    }
    public override void Fire()
    {
        if (nowWeapon != null)
        {
            nowWeapon.Fire();
            StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));  // ������
            isAiming = false;
            aimUIPanel.SetActive(false);
        }
    }
    public override void Dead()
    {
        

       
      

        // �ص�begin����
        //SceneManager.LoadScene("begin");
    }

    public override void Wound(TankBase other)
    {
        base.Wound(other);
        GamePanel.Instance.UpdateHP(this.MaxHP, this.HP);

       if(HP>1) StartCoroutine(CameraShake(woundShakeDuration, woundShakeMagnitude));
    }

  
    public void ChangeWeapon(GameObject weapon)
    {
        if (nowWeapon != null)
        {
            Destroy(nowWeapon.gameObject);
            nowWeapon = null;
        }

        //�л�����
        GameObject weaponObj = Instantiate(weapon, WeaponPos,false);
        nowWeapon = weaponObj.GetComponent<WeaponObj>();

        nowWeapon.SetFather(this);
    }
    IEnumerator CameraShake(float duration, float magnitude)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;
            mainCam.transform.position += randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
    }

}
