using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : BasePanel<GamePanel>
{
    public CustomGUILabel Score;
    public CustomGUILabel Times;
    public CustomGUILabel Def;
    public CustomGUIButton Quit;
    public CustomGUIButton Back;
    public CustomGUIButton Setting;
    public CustomGUITexture HP;
    public CustomGUITexture HPBK;
    [HideInInspector]
    public int NowScore = 0 ;
    public float hpW = 350;
    [HideInInspector]
    public float nowtime = 0;
    private int time ;
    public float hpMaxW;  // 初始最大宽度，只赋值一次
    public float reloadMaxH=386.4f;  
    public CustomGUITexture reloadpic;
    public CustomGUITexture ammoIcon;    // 基础备弹图标
    public float ammoIconSpacing = 50f;  // 每个图标之间的间距
    private int lastAmmo = -1;  // 缓存上次弹药数
    private List<CustomGUITexture> ammoIcons = new List<CustomGUITexture>();
    public CustomGUITexture cooldownPic;  // 冷却条图片
    public float cooldownMaxW = 466.43f;     // Inspector里手动设置最大高度
    public string timelabel;
    public string scorelabel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DynamicGI.UpdateEnvironment();
    }
    void Update()
    {
        nowtime += Time.deltaTime;
        time = (int)nowtime;
        Times.content.text = "";
       
        if (time / 3600 > 0) Times.content.text += time / 3600 + "时";
        if (time % 3600 / 60 > 0 || Times.content.text != "") Times.content.text += time % 3600 / 60 + "分";
        Times.content.text += time % 60 + "秒";
        timelabel = Times.content.text;
        if (Input.GetKeyDown(KeyCode.Escape))
            {
               Time.timeScale = 0; 
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
            EscPanel.Instance.Showme(); 
            EscPanel.Instance.isShow = true ; 

            }

        
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !EscPanel.Instance.isShow)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    /// <summary>
    ///加分方法
    /// </summary>
    public void AddScore(int score) 
    {
        NowScore += score;
        Score.content.text = NowScore.ToString();
        scorelabel = Score.content.text;
    }
    public void UpdateHP(int MaxHp,int NowHP)
    {
        HP.guiPos.width = (float)NowHP/ (float)MaxHp * (float)hpW;
        HPBK.guiPos.width = hpW+ 18;
    }
    public void UpdateDef(int def)
    {
       Def.content.text = def.ToString();
    }
    public void UpdateReload(float currentReloadTime, float maxReloadTime, bool isReloading)
    {
        if (reloadpic != null)
        {
            if (!isReloading)
            {
                // 满弹直接隐藏
                reloadpic.gameObject.SetActive(false);

            }
            else
            {
                reloadpic.gameObject.SetActive(true);
                float height = reloadMaxH * currentReloadTime / maxReloadTime;
                reloadpic.guiPos.height = height;
            }
        }
    }
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (currentAmmo == lastAmmo) return;  // 没变化直接返回
        lastAmmo = currentAmmo;

        // 清除多余图标
        for (int i = ammoIcons.Count - 1; i >= 0; i--)
        {
            Destroy(ammoIcons[i].gameObject);
        }
        ammoIcons.Clear();

        // 根据当前弹药数生成图标
        for (int i = 0; i < currentAmmo; i++)
        {
            if (i == 0)
            {
                ammoIcon.gameObject.SetActive(true);
            }
            else
            {
                GameObject newIcon = Instantiate(ammoIcon.gameObject, ammoIcon.transform.parent);
                CustomGUITexture iconTex = newIcon.GetComponent<CustomGUITexture>();
                iconTex.guiPos.pos.y = ammoIcon.guiPos.pos.y + ammoIconSpacing * i;
                ammoIcons.Add(iconTex);
            }
        }

        if (currentAmmo <= 0)
            ammoIcon.gameObject.SetActive(false);
    }
    public void UpdateCooldown(float currentCooldown, float maxCooldown)
    {
        if (cooldownPic == null) return;

        if (currentCooldown <= 0)
        {
            cooldownPic.gameObject.SetActive(false);
        }
        else
        {
            cooldownPic.gameObject.SetActive(true);
            float width = cooldownMaxW * currentCooldown / maxCooldown;
            cooldownPic.guiPos.width = width;

        }
    }
}
