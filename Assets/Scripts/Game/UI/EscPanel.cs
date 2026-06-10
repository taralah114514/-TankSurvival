using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EscPanel : BasePanel<EscPanel>
{
    public CustomGUIButton Quit;
    public CustomGUIButton Back;
    public CustomGUIButton Setting;
    public bool isShow;
    //private bool isPaused = false;
    void Start()
    {   
        Quit.clickEvent += () =>
        {
            EscConfirmPanel.Instance.Showme(); 
        };
        Back.clickEvent += () =>
        {
            ResumeGame();
        };
        Setting.clickEvent += () =>
        {
            Hideme();
            SettingPanel.Instance.Showme();
        };
        Hideme();

    }
     
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isShow = false;
        }
    }
    public override void Hideme()
    {
        base.Hideme();
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        //isPaused = true;

        // 显示面板
       
        // 暂停游戏时间
        //if (pauseGame)
        //{
        //    Time.timeScale = 0f;
        //}

        // 解锁鼠标
        //if (lockCursor)
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}

        // 触发暂停事件
        //OnGamePaused?.Invoke();
    }
    public void ResumeGame()
    {
        //isPaused = false;

        // 隐藏面板
       Hideme() ;

        // 恢复游戏时间
        //if (pauseGame)
        //{
        //    Time.timeScale = 1f;
        //}

        // 锁定鼠标
        //if (lockCursor)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}

        // 触发继续事件
        //OnGameResumed?.Invoke();
    }
   
}
