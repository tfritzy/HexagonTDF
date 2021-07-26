using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectTowerMenu : MonoBehaviour
{
    public AttackTower TargetTower { get; private set; }
    private float setActiveTime;
    private Text upgradeCostText;

    void Start()
    {
        InputManager.InputWasMade += InformOfClickElsewhere;
    }

    void Update()
    {
        Follow();
    }

    public void SetTargetTower(AttackTower targetTower)
    {
        this.TargetTower = targetTower;
        this.setActiveTime = Time.time;

        if (upgradeCostText == null)
        {
            upgradeCostText = this.transform.Find("UpgradeButton").Find("Count Box").Find("Text").GetComponent<Text>();
        }

        upgradeCostText.text = this.TargetTower.UpgradeCost.Costs[ResourceType.Gold].ToString();
        this.TargetTower.ShowRangeCircle();
    }

    private void Follow()
    {
        if (TargetTower == null)
        {
            Disable();
        }

        this.transform.position = Managers.Camera.WorldToScreenPoint(TargetTower.transform.position);
    }

    public void InformOfClickElsewhere(object sender, EventArgs e)
    {
        if (Time.time > setActiveTime + .25f && this.gameObject.activeSelf)
        {
            Disable();
        }
    }

    void OnDestroy()
    {
        InputManager.InputWasMade -= InformOfClickElsewhere;
    }

    public void Upgrade()
    {
        TargetTower.Upgrade();
        upgradeCostText.text = TargetTower.UpgradeCost.Costs[ResourceType.Gold].ToString();
    }

    public void Disable()
    {
        this.TargetTower?.HideRangeCircle();
        this.gameObject.SetActive(false);
    }
}
