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
    public int IslandIndex;
    public int Id;

    private float powerMultiplier;
    private Text indicatorText;
    private GameObject powerIndicatorInst;
    private const float lineHeight = 0.68f;

    public void Setup(float powerMultiplier, Vector2Int pos, int islandIndex, int id)
    {
        this.IslandIndex = islandIndex;
        this.Id = id;
        Alliance = Helpers.GetAlliance(islandIndex, id);

        // if (powerIndicatorInst == null)
        // {
        //     InitPowerIndicator();
        // }
        // this.indicatorText.text = ((int)(powerMultiplier * 10)).ToString();

        this.powerMultiplier = powerMultiplier;
        this.Position = pos;
        this.gameObject.name = "Fortress " + id;
        SetColor();
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

    private void SetColor()
    {
        transform.Find("Body").GetComponent<MeshRenderer>().materials[1].color = Constants.AllianceColorMap[this.Alliance];

        foreach (ParticleSystem system in this.GetComponentsInChildren<ParticleSystem>())
        {
            var main = system.main;
            main.startColor = Constants.AllianceColorMap[this.Alliance];

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
            system.GetParticles(particles, system.particleCount, 0);
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].startColor = Constants.AllianceColorMap[this.Alliance];
            }
            system.SetParticles(particles);
        }
    }

    public bool Interact()
    {
        Select();
        return true;
    }

    public void Select()
    {
        GameState.LevelPowerMultiplier = this.powerMultiplier;
        GameState.SelectedSegment = Managers.OverworldManager.Island;
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
