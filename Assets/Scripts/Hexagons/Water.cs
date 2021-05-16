using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Hexagon
{
    public override HexagonType Type => HexagonType.Water;
    public override bool IsBuildable => false;
}
