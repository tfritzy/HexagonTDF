using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureProgressBar : MonoBehaviour
{
    private Text text;
    private Image fillBar;

    void Start()
    {
        fillBar = transform.Find("Bar").Find("Fill").GetComponent<Image>();
        text = transform.Find("Level Frame").Find("Text").GetComponent<Text>();
    }

    public void SetValue(float fillPercent)
    {
        text.text = (int)(fillPercent * 100) + "%";
        fillBar.fillAmount = fillPercent;
    }
}
