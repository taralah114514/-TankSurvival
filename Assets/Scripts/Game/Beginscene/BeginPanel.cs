using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginPanel :BasePanel<BeginPanel>
{   
    public CustomGUIButton btnBeginGame;
    public CustomGUIButton btnSetting;
    public CustomGUIButton btnRanking;
    public CustomGUIButton btnExit;
    void Start()
    {
        btnBeginGame.clickEvent += () =>
        {
            SceneManager.LoadScene("tankScene");
        };
        btnSetting.clickEvent += () =>
        {
            Hideme();
            SettingPanel.Instance.Showme();
        };
        btnRanking.clickEvent += () =>
        {
            Hideme();
            RankPanel.Instance.Showme();
        };
        btnExit.clickEvent += () =>
        {

            // 清空 Unity 所有 PlayerPrefs 保存的数据
            //PlayerPrefs.DeleteAll();
            
            Application.Quit();
        };
    }
}
