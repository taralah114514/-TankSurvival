using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager instance=new DataManager();
    public static DataManager Instance => instance;
    public  MusicData Musicdata;
    public RankList Ranklist;
    public DataManager()
    {   
        //��ʼ����Ϸ����
        Musicdata = PlayerPrefsDataMgr.Instance.LoadData(typeof(MusicData), "Music")as MusicData;
     
        if (!Musicdata.notFirst) 
        {
            Musicdata.notFirst=true;
            Musicdata.isopenMusic= true;
            Musicdata.isopenSound=true;
            Musicdata.soundValue= 0.5f;
            Musicdata.musicValue= 0.5f;
            PlayerPrefsDataMgr.Instance.SaveData(Musicdata, "Music");
        }
        Ranklist = PlayerPrefsDataMgr.Instance.LoadData(typeof(RankList), "Rank") as RankList;
        
        
    }
    public void AddRankData(string name,int score,float time) 
    {
        Ranklist.list.Add(new RankData(name,score,time));
        Ranklist.list.Sort((a, b) => a.time > b.time ? -1:1);
        for (int i = Ranklist.list.Count-1; i >= 7; i--)
        { Ranklist.list.RemoveAt(i); }
        PlayerPrefsDataMgr.Instance.SaveData(Ranklist, "Rank");
       
    }

}
