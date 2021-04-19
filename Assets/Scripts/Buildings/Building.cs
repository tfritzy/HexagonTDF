using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Character
{
    public Sprite Icon { get => Prefabs.BuildingIcons[Type]; }
    public abstract BuildingType Type { get; }
    public Vector2Int Position { get; set; }
    public override int StartingHealth => int.MaxValue;
    public ResourceTransaction BuildCost => new ResourceTransaction(this.Power, this.CostRatio);
    public abstract Dictionary<ResourceType, float> CostRatio { get; }
    public override VerticalRegion Region => VerticalRegion.Ground;

    public void Initialize(Vector2Int position)
    {
        this.Position = position;
        Managers.Map.AddBuilding(this);

        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = Constants.Materials.Normal;
        }
    }
}
