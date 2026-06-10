using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankPanel : BasePanel<RankPanel>
{
    public CustomGUIButton CloseButton;
    private List<CustomGUILabel> Ranks =new List<CustomGUILabel>();
    private List<CustomGUILabel> Names =new List<CustomGUILabel>();
    private List<CustomGUILabel> Scores=new List<CustomGUILabel>();
    private List<CustomGUILabel> Times =new List<CustomGUILabel>();

    
    void Start()
    {

        for(int i = 1; i < 8; i++) 
        {
        Ranks.Add(this.transform.Find("Ranks/Label (6)"+i).GetComponent<CustomGUILabel>());
        Names.Add(this.transform.Find("Name/Label (14)"+i).GetComponent<CustomGUILabel>());
        Scores.Add(this.transform.Find("Scores/Label (15)"+i).GetComponent<CustomGUILabel>());
        Times.Add(this.transform.Find("Times/Label (16)"+i).GetComponent<CustomGUILabel>());
        }

        if (!PlayerPrefs.HasKey("isFirstLaunch"))
        {
            // ��һ�����У���ʼ������
            PlayerPrefs.SetInt("isFirstLaunch", 1);
            PlayerPrefs.Save();

            // ������д��һ�����еĳ�ʼ���߼�
            InitFirstLaunch();
        }

        CloseButton.clickEvent += () =>
        {
            Hideme();
            BeginPanel.Instance.Showme();
        };

        Hideme();
       
    }
    public void UpdatePanelInfo()
    {
        List<RankData> list = DataManager.Instance.Ranklist.list;

        for (int i = 0; i < list.Count; i++) 
        {
          
           Names[i].content.text=list[i].name;
           Scores[i].content.text = list[i].score.ToString();
           int time =(int)list[i].time;
           Times[i].content.text = "";
         
           if (time / 3600 > 0) Times[i].content.text += time / 3600 + "ʱ";
           if(time % 3600/60 >0 || Times[i].content.text != "") Times[i].content.text += time % 3600/60 + "��";
           Times[i].content.text += time %  60 + "��";
            

        }
    }
    void InitFirstLaunch()
    {
        DataManager.Instance.AddRankData("����", 114514, 8);
        DataManager.Instance.AddRankData("test2", 350234, 8432);
        DataManager.Instance.AddRankData("69", 91, 1);
    }
    public override void Showme()
    {
        base.Showme();
       
        UpdatePanelInfo();
    }
}
