using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingPanel : BasePanel<SettingPanel>
{
    public CustomGUISlider SliderMusic;
    public CustomGUISlider SliderSound;
    public CustomGUIToggle ToggleMusic;
    public CustomGUIToggle ToggleSound;

    public CustomGUIButton ButtonClose;
    void Start()
    {
       
        SliderMusic.changeValue += (value) =>
        { BKMusic.Instance.ChangeValue(value);};

        SliderSound.changeValue += (value) =>
        {
            //音效

        };
       
        ToggleMusic.changeValue += (value) =>
        {BKMusic.Instance.Openornot(!value);};

        ToggleSound.changeValue += (value) =>
        {
            //音效

        };
        ButtonClose.clickEvent += () =>
        {
            SavePanelInfo();
            Hideme();
            if (SceneManager.GetActiveScene().name == "begin")
            {
                Debug.Log("当前是场景1");
                BeginPanel.Instance.Showme();
            }
            if (SceneManager.GetActiveScene().name == "tankScene")
            {
                Debug.Log("当前是场景2");
             
                EscPanel.Instance.Showme();
            }


        };

        Hideme();
    }
    public void UpdatePanelInfo()
    {
        MusicData date = DataManager.Instance.Musicdata;
        SliderMusic.nowValue = date.musicValue;
        SliderSound.nowValue = date.soundValue;
        ToggleMusic.isSel = date.isopenMusic;
        ToggleSound.isSel = date.isopenSound;
    }
    public void SavePanelInfo() 
    {
        DataManager.Instance.Musicdata.musicValue = SliderMusic.nowValue;
        DataManager.Instance.Musicdata.soundValue = SliderSound.nowValue;
        DataManager.Instance.Musicdata.isopenMusic = ToggleMusic.isSel;
        DataManager.Instance.Musicdata.isopenSound = ToggleSound.isSel;
        PlayerPrefsDataMgr.Instance.SaveData(DataManager.Instance.Musicdata, "Music");

    }
    public override void Showme()
    {
        base.Showme();
        UpdatePanelInfo();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SavePanelInfo();
            Hideme();
            if (SceneManager.GetActiveScene().name == "begin")
            {
                BeginPanel.Instance.Showme();
            }
            if (SceneManager.GetActiveScene().name == "tankScene")
            {
                EscPanel.Instance.Showme();
            }
        }
    }
}
