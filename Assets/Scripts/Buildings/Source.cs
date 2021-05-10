using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Source : AttackTower
{
    public override BuildingType Type => BuildingType.Source;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override int StartingHealth => 50;
    public override float Cooldown => AttackSpeed.Slow;
    public override int Damage => 5;
    public override float Range => RangeOptions.Medium;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;

    private Text healthText;

    protected override void Setup()
    {
        base.Setup();
        this.healthText = Managers.Canvas.Find("HealthUI").Find("Circle").Find("Count Box").Find("Text").GetComponent<Text>();
        this.healthText.text = this.Health.ToString();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        healthText.text = this.Health.ToString();
    }
}
