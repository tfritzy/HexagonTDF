using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectTowerMenu : MonoBehaviour
{
    public AttackTower TargetTower { get; private set; }
    private float setActiveTime;
    private Text upgradeCostText;
    private float DisableTime = float.MaxValue;
    private Color DisabledColor = ColorExtensions.Create("#787878FF");
    private ButtonComponents UpgradeButton;
    private bool awaitingConfirmation;

    private class ButtonComponents
    {
        public Color OriginalColor;
        public Sprite OriginalIcon;
        public Image Icon;
        public Image Outline;
        public Button Button;
    }

    void Start()
    {
        this.UpgradeButton = ParseButton("UpgradeButton");
    }

    void Update()
    {
        Follow();

        if (Time.time > DisableTime)
        {
            DisableLogic();
        }

        SetButtonStatuses();
    }

    public void SetTargetTower(AttackTower targetTower)
    {
        this.DisableTime = float.MaxValue;
        this.TargetTower?.HideRangeCircle();
        this.TargetTower = targetTower;
        this.setActiveTime = Time.time;

        if (upgradeCostText == null)
        {
            upgradeCostText = this.transform.Find("UpgradeButton").Find("Count Box").Find("Text").GetComponent<Text>();
        }

        upgradeCostText.text = this.TargetTower.UpgradeCost.Costs[ResourceType.Gold].ToString();
        this.TargetTower.ShowRangeCircle();
    }

    private ButtonComponents ParseButton(string buttonName)
    {
        Transform upgradeButton = this.transform.Find(buttonName);
        Image upgradeButtonImage = upgradeButton.Find("Icon").GetComponent<Image>();
        return new ButtonComponents
        {
            Icon = upgradeButtonImage,
            OriginalColor = upgradeButtonImage.color,
            OriginalIcon = upgradeButtonImage.sprite,
            Outline = upgradeButton.Find("Inner Glow").GetComponent<Image>(),
            Button = upgradeButton.GetComponent<Button>(),
        };
    }

    private void ResetButton(ButtonComponents button)
    {
        button.Icon.sprite = button.OriginalIcon;
        button.Outline.color = button.OriginalColor;
        button.Icon.color = button.OriginalColor;
    }

    private void SetButtonStatuses()
    {
        if (TargetTower.UpgradeCost.CanFulfill())
        {
            UpgradeButton.Icon.color = UpgradeButton.OriginalColor;
            UpgradeButton.Outline.color = UpgradeButton.OriginalColor;
            UpgradeButton.Button.enabled = true;
        }
        else
        {
            UpgradeButton.Icon.color = DisabledColor;
            UpgradeButton.Outline.color = DisabledColor;
            UpgradeButton.Button.enabled = false;
        }
    }

    private void Follow()
    {
        if (TargetTower == null)
        {
            Disable();
        }

        this.transform.position = Managers.Camera.WorldToScreenPoint(TargetTower.Position);
    }

    public void Upgrade()
    {
        if (!awaitingConfirmation)
        {
            UpgradeButton.Icon.sprite = Prefabs.UIIcons[UIIconType.Accept];
            awaitingConfirmation = true;
        }
        else
        {
            TargetTower.Upgrade();
            upgradeCostText.text = TargetTower.UpgradeCost.Costs[ResourceType.Gold].ToString();
            Disable();
        }
    }

    private void DisableLogic()
    {
        awaitingConfirmation = false;
        ResetButton(this.UpgradeButton);
        this.TargetTower?.HideRangeCircle();
        this.gameObject.SetActive(false);
        this.transform.position = new Vector3(float.MaxValue / 2, float.MaxValue / 2, float.MaxValue / 2);
    }

    public void Disable()
    {
        this.DisableTime = Time.time + .001f;
    }
}
