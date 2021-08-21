using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldLevelSelect : MonoBehaviour, Interactable
{
    public bool Interact()
    {
        Select();
        return true;
    }

    public void Select()
    {
        print("Load level");
        SceneManager.LoadScene("Level");
    }
}
