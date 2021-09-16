using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverworldFortress : MonoBehaviour, Interactable
{
    public GameObject PowerIndicator;
    public Vector2Int Position;
    public List<OverworldFortress> ReliesOn { get; private set; }
    public GameObject Link;
    public Alliances Alliance;

    private float powerMultiplier;
    private Text indicatorText;
    private GameObject powerIndicatorInst;
    private const float lineHeight = 0.68f;

    public void Setup(float powerMultiplier, Vector2Int pos)
    {
        if (powerIndicatorInst == null)
        {
            InitPowerIndicator();
        }

        this.powerMultiplier = powerMultiplier;
        this.indicatorText.text = ((int)(powerMultiplier * 10)).ToString();
        this.Position = pos;
        this.gameObject.name = "Fortress " + this.Position;
    }

    private void InitPowerIndicator()
    {
        powerIndicatorInst = Instantiate(PowerIndicator, Managers.Canvas);
        powerIndicatorInst.transform.SetParent(Managers.Canvas);
        UIElementFollowObject follow = powerIndicatorInst.GetComponent<UIElementFollowObject>();
        follow.ObjectToFollow = this.gameObject;
        follow.Offset = new Vector3(0, 1f, 0);
        this.indicatorText = powerIndicatorInst.transform.Find("Text").GetComponent<Text>();
    }


    public bool Interact()
    {
        Select();
        return true;
    }

    public void Select()
    {
        GameState.LevelPowerMultiplier = this.powerMultiplier;
        // GameState.SelectedSegment = Managers.OverworldManager.GetSegment(this.Position);
        Managers.LoadingMenu.LoadScene("Level");
    }

    void OnDisable()
    {
        if (this.powerIndicatorInst != null)
        {
            this.powerIndicatorInst.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (this.powerIndicatorInst != null)
        {
            this.powerIndicatorInst.SetActive(true);
        }
    }
}
