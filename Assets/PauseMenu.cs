using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        Managers.PauseMenu.gameObject.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void Retreat()
    {
        SceneManager.LoadScene("Overworld");
    }
}
