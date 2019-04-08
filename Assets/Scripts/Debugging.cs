using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugging : MonoBehaviour
{
    public GameObject panel;
    public Button debugToggleButton;
    public Text debugText;
    public static string debugStr;

    private void Update()
    {
        if(panel != null && debugToggleButton != null && debugText != null)
            debugText.text = debugStr;
    }

    public static void SetDebugText(string str)
    {
        debugStr = str;
    }

    public void ToggleDebugPanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
