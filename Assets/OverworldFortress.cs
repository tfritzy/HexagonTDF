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

    private float powerMultiplier;
    private Text indicatorText;
    private GameObject powerIndicatorInst;

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

    public void CreateLinks()
    {
        foreach (LineRenderer lr in this.GetComponentsInChildren<LineRenderer>())
        {
            Destroy(lr.gameObject);
        }

        ReliesOn = new List<OverworldFortress>();
        int myIndex = Managers.OverworldManager.Fortresses.IndexOf(this);
        print(myIndex);

        if (myIndex == 0 && Managers.OverworldManager.Fortresses.Count > 2)
        {
            Managers.OverworldManager.Fortresses[1].CreateLinks();
            Managers.OverworldManager.Fortresses[2].CreateLinks();
        }
        else
        {
            List<int> targetIndices = new List<int> { myIndex - 1, myIndex - 2 };
            if ((int)Position.magnitude % 2 == 1)
            {
                targetIndices.RemoveAt(0);
            }

            foreach (int targetIndex in targetIndices)
            {
                if (isLinkValid(targetIndex))
                {
                    print(string.Join(", ", Managers.OverworldManager.Fortresses));
                    ReliesOn.Add(Managers.OverworldManager.Fortresses[targetIndex]);
                    LineRenderer link = Instantiate(
                        this.Link,
                        this.transform.position,
                        new Quaternion(),
                        this.transform)
                            .GetComponent<LineRenderer>();
                    link.positionCount = 2;
                    print($"{myIndex} -> {targetIndex}");
                    print($"{ReliesOn.Last().gameObject.name}");
                    link.SetPosition(0, this.transform.position);
                    link.SetPosition(1, ReliesOn.Last().transform.position);
                }
            }
        }
    }

    private bool isLinkValid(int fortressIndex)
    {
        if (fortressIndex < 0 || fortressIndex >= Managers.OverworldManager.Fortresses.Count)
        {
            return false;
        }

        return true;
    }

    public bool Interact()
    {
        Select();
        return true;
    }

    public void Select()
    {
        GameState.LevelPowerMultiplier = this.powerMultiplier;
        GameState.SelectedSegment = Managers.OverworldManager.GetSegment(this.Position);
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
