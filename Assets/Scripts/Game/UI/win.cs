
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class win : BasePanel<win>
{
    public CustomGUIButton btnCancel;
   public  CustomGUILabel a;
   public  CustomGUILabel time;
   public  CustomGUIInput name1;
    // Start is called before the first frame update
    void Start()
    {
        btnCancel.clickEvent += () =>
        {
            Time.timeScale = 1;
            DataManager.Instance.AddRankData(name1.content.text,GamePanel.Instance.NowScore, GamePanel.Instance.nowtime);
            SceneManager.LoadScene("begin");
         
           
        };
       

        Hideme();
    }
    void Update()
    {
        a.content.text = GamePanel.Instance.scorelabel;
        time.content.text = GamePanel.Instance.timelabel;
    }

}
