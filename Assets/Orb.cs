using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : Building
{
    public override BuildingType Type => BuildingType.Orb;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override int StartingHealth => 100;
    public override float Power => int.MaxValue;
    public bool IsBeingConverted;
    public float ConversionTimeRequired;
    private float conversionProgress;

    protected override void Setup()
    {
        base.Setup();

        ConversionTimeRequired = 300f;

        // TODO have captruing begin when mage contacts.
        IsBeingConverted = true;
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        conversionProgress += Time.deltaTime;
        Managers.CaptureProgressBar.SetValue(conversionProgress / ConversionTimeRequired);
    }
}
