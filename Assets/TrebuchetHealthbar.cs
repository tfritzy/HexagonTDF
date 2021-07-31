using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrebuchetHealthbar : MonoBehaviour
{
    private Text text;
    private Image fillBar;

    void Start()
    {
        fillBar = transform.Find("Bar").Find("Fill").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
    }

    public void SetValue(float currentVal, float maxVal)
    {
        text.text = $"{currentVal} / {maxVal}";
        fillBar.fillAmount = currentVal / maxVal;
    }
}
