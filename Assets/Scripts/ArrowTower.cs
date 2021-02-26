using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowTower : Building
{
    public override Sprite Icon => Prefabs.BuildingIcons[Type];
    public override BuildingType Type => BuildingType.ArrowTower;
}
