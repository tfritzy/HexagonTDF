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
        BuildingType.ArcherFortress,
        BuildingType.ArcherFortress,
        BuildingType.SparkTower,
        BuildingType.LightningTower,
        BuildingType.ChainLightningTower,
        BuildingType.FireTower,
        BuildingType.FireTower,
        BuildingType.MeteorTower,
        BuildingType.EarthTower,
        BuildingType.CrystalAccelerator,
        BuildingType.FrigidShard,
    };
}
