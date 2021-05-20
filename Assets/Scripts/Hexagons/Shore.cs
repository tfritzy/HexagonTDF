using System;
using System.Collections.Generic;
using UnityEngine;

public class Shore : Hexagon
{
    public override HexagonType Type => HexagonType.Shore;
    public override bool IsBuildable => true;
    public override bool IsWalkable => true;
}