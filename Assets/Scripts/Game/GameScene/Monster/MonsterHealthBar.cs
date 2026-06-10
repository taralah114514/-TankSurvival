using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealthBar : MonoBehaviour
{
    [Header("血条物体")]
    public Transform hpBar;
    public Transform bufferBar;
    public Transform bgBar;

    [Header("设置")]
    //public float maxWidth = 1f;
    private float initWidth;


    public float bufferDelay = 2f;
    public float bufferSpeed = 2f;
    public float showDistance = 10f;   // 显示血条的距离
    public float hitShowDuration = 5f; // 被击中后显示时长

    private Transform player;
    private Transform playerCam;

    private float currentHP;
    private float maxHP;
    private float bufferHP;
    private float bufferTimer = 0f;
    private bool bufferWaiting = false;
    private PlayerObject playerObj;  // 缓存玩家组件
    private float hitTimer = 0f; // 被击中后的计时器

    void Awake()
    {
        // 先在Awake里记录初始宽度
        if (hpBar != null) initWidth = hpBar.localScale.x;
    }

    void Start()
    {
        playerObj = FindObjectOfType<PlayerObject>();
        if (playerObj != null) player = playerObj.transform;
        playerCam = Camera.main.transform;
        bufferHP = 0f;
        SetVisible(false);
    }


    void Update()
    {
        if (playerObj == null || playerCam == null) return;

        transform.LookAt(transform.position + playerCam.forward);

        // 直接用缓存的playerObj，不用每帧GetComponent
        bool isAiming = playerObj.isAiming;
        float dist = Vector3.Distance(transform.position, player.position);
        bool inRange = dist <= showDistance;

        if (hitTimer > 0f) hitTimer -= Time.deltaTime;

        SetVisible(isAiming || inRange || hitTimer > 0f);

        // 缓冲条逻辑
        if (bufferWaiting)
        {
            bufferTimer += Time.deltaTime;
            if (bufferTimer >= bufferDelay)
            {
                bufferWaiting = false;
                bufferTimer = 0f;
            }
        }
        else if (bufferHP > currentHP)
        {
            bufferHP = Mathf.MoveTowards(bufferHP, currentHP, bufferSpeed * Time.deltaTime);
            UpdateBufferBar();
        }
    }

    public void UpdateHP(float current, float max)
    {
        maxHP = max;
        currentHP = current;

        // 确保initWidth已经有值
        if (initWidth <= 0f && hpBar != null)
            initWidth = hpBar.localScale.x;

        if (bufferHP <= 0f) bufferHP = current;

        UpdateHPBar();
        UpdateBufferBar();
        bufferWaiting = true;
        bufferTimer = 0f;
    }

    // 被击中时调用，触发5秒显示
    public void OnHit(float current, float max)
    {
        hitTimer = hitShowDuration;
        UpdateHP(current, max);
    }

    private void UpdateHPBar()
    {
        if (hpBar == null) return;
        float ratio = currentHP / maxHP;
        Vector3 scale = hpBar.localScale;
        scale.x = initWidth * ratio;
        hpBar.localScale = scale;
        Vector3 pos = hpBar.localPosition;
        pos.x = (ratio - 1f) * initWidth * 0.5f;
        hpBar.localPosition = pos;
    }

    private void UpdateBufferBar()
    {
        if (bufferBar == null) return;
        float ratio = bufferHP / maxHP;
        Vector3 scale = bufferBar.localScale;
        scale.x = initWidth * ratio;
        bufferBar.localScale = scale;
        Vector3 pos = bufferBar.localPosition;
        pos.x = (ratio - 1f) * initWidth * 0.5f;
        bufferBar.localPosition = pos;
    }

    private void SetVisible(bool visible)
    {
        if (hpBar != null) hpBar.gameObject.SetActive(visible);
        if (bufferBar != null) bufferBar.gameObject.SetActive(visible);
        if (bgBar != null) bgBar.gameObject.SetActive(visible);
    }
}
