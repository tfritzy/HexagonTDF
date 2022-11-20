using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour
{
    public GameObject FillObject;
    public GameObject StatusText;

    private Image fill;
    private Text status;

    public void SetStatus(string stateName, float statePercent)
    {
        if (this.fill == null || this.status == null)
        {
            this.fill = FillObject.GetComponent<Image>();
            this.status = StatusText.GetComponent<Text>();
        }

        this.fill.fillAmount = statePercent;
        this.status.text = stateName + GetPeriods();
    }

    private string GetPeriods()
    {
        int timeMod = (int)Time.time % 3;
        switch (timeMod)
        {
            case (0):
                return ".";
            case (1):
                return "..";
            case (2):
                return "...";
            default:
                return ".";
        }
    }
}
