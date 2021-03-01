﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Hexagon
{
    public override HexagonType Type => HexagonType.Stone;

    public override bool IsBuildable => true;
}
