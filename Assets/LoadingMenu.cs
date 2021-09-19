using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public string DefaultSceneToLoad;

    private AsyncOperation loadingOperation;
    private Image loadingBar;
    private string sceneName;

    void Start()
    {
        this.loadingBar = this.transform.Find("LoadingBar").Find("FillBar").GetComponent<Image>();

        if (!string.IsNullOrEmpty(DefaultSceneToLoad))
        {
            LoadScene(DefaultSceneToLoad);
        }
        else
        {
            Managers.LoadingMenu.gameObject.SetActive(false); // Update reference in managers and disable self.
        }
    }

    public void LoadScene(string sceneName)
    {
        this.gameObject.SetActive(true);
        this.sceneName = sceneName;
        loadingOperation = SceneManager.LoadSceneAsync(this.sceneName);
    }

    void FixedUpdate()
    {
        if (loadingOperation != null)
        {
            this.loadingBar.transform.localScale = new Vector3(loadingOperation.progress, 1, 1);
        }
    }
}
