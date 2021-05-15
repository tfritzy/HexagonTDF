using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Hexagon
{
    public override HexagonType Type => HexagonType.Water;
    public override bool IsBuildable => false;

    protected override void Setup()
    {
        base.Setup();

        // Enemies should fall through water.
        this.transform.Find("Collider").GetComponent<MeshCollider>().enabled = false;
    }
}
