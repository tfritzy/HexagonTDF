using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerBuildMenu : BuildMenu
{
    private readonly List<BuildingType> buildingTypes = new List<BuildingType>()
    {
        BuildingType.ArrowTower,
        BuildingType.MageTower,
        BuildingType.RockTower,
        BuildingType.LightningTower,
        BuildingType.FireTower,
        BuildingType.EarthTower,
    };

    public override List<BuildingType> BuildingTypes
    {
        get { return buildingTypes; }
    }
}
