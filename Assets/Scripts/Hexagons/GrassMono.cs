using UnityEngine;

public class GrassMono : HexagonMono
{
    protected override void SetHexBodyColor()
    {
        base.SetHexBodyColor();
        for (int i = 0; i < 6; i++)
        {
            MeshRenderer grass = transform.Find("GrassEdge" + i).GetComponent<MeshRenderer>();
            grass.material.color = this.ColorAfterVariance;
        }
    }
}