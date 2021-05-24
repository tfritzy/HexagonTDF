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
        BuildingType.Spikes,
        BuildingType.ArcherFortress,
        BuildingType.SparkTower,
        BuildingType.ChainLightningTower,
        BuildingType.LightningTower,
        BuildingType.FireTower,
        BuildingType.FlamethrowerTower,
        BuildingType.MeteorTower,
        BuildingType.EarthTower,
        BuildingType.CrystalAccelerator,
        BuildingType.FrigidShard,
    };
}
