﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forrest : ObstacleHexagon
{
    public override Biome Biome => Biome.Forrest;
    public override Color BaseColor => ColorExtensions.Create("#306847");
    public override float ObstacleChance => .3f;

    public override GameObject GetObstacle()
    {
        return Managers.Prefabs.Tree;
    }
}
