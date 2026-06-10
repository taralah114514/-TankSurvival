using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscConfirmPanel : BasePanel<EscConfirmPanel> 
{ 
    public CustomGUIButton btnCancel;
    public CustomGUIButton btnOk;
    // Start is called before the first frame update
    void Start()
    {
        btnCancel.clickEvent += () =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("begin");
           
        };
        btnOk.clickEvent += () =>
        {
            Hideme();
            EscPanel.Instance.Showme();
        };
       
        Hideme();
    }

    
}
