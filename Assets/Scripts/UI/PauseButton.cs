using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void Pause()
    {
        Time.timeScale = 0f;
        Managers.PauseMenu.gameObject.SetActive(true);
    }
}
