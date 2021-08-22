using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    private AsyncOperation loadingOperation;
    private Image loadingBar;
    private Text percent;
    private int numFramesTillLoad;
    private string sceneName;

    void Awake()
    {
        this.loadingBar = this.transform.Find("LoadingBar").Find("Mask").Find("FillBar").GetComponent<Image>();
        this.percent = this.transform.Find("LoadingBar").Find("LoadPercent").GetComponent<Text>();
        Managers.LoadingMenu.gameObject.SetActive(false); // Update reference in managers and disable self.
    }

    public void LoadScene(string sceneName)
    {
        // Wait a frame to start loading to make loading window appear immediately.
        this.gameObject.SetActive(true);
        numFramesTillLoad = 5;
        this.sceneName = sceneName;
    }

    void Update()
    {
        if (sceneName != null)
        {
            if (sceneName != null && numFramesTillLoad > 0)
            {
                numFramesTillLoad -= 1;
            }
            else
            {
                loadingOperation = SceneManager.LoadSceneAsync(this.sceneName);
                sceneName = null;
            }
        }

        if (loadingOperation != null)
        {
            this.loadingBar.fillAmount = loadingOperation.progress;
            this.percent.text = $"{(int)(loadingOperation.progress * 100)}%";
        }
    }
}
