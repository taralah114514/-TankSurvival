using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimUI : MonoBehaviour
{
    public GameObject aimUIPanel;  // Õœ»Î√È◊ºUI√Ê∞Â

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            aimUIPanel.SetActive(true);

        if (Input.GetMouseButtonUp(1))
            aimUIPanel.SetActive(false);
    }
}