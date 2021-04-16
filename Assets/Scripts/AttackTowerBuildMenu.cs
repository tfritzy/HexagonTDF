using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerBuildMenu : BuildMenu
{
    public override List<BuildingType> BuildingTypes
    {
        get { return buildingTypes; }
    }

    private readonly List<BuildingType> buildingTypes = new List<BuildingType>()
    {
        BuildingType.ArcherTower,
        BuildingType.LightningTower,
        BuildingType.FireTower,
        BuildingType.EarthTower,
        BuildingType.ArcherFortress,
        BuildingType.CrystalAccelerator,
        BuildingType.FrigidShard,
    };
}
