using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Character
{
    public Sprite Icon { get => Prefabs.BuildingIcons[Type]; }
    public abstract BuildingType Type { get; }
    public Vector2Int GridPosition { get; set; }
    public ResourceTransaction BuildCost => new ResourceTransaction(this.Power, costRatio);
    public override VerticalRegion Region => VerticalRegion.Ground;
    public virtual bool IsWalkable => false;
    public abstract bool IsVillageBuilding { get; }
    private static Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float> { { ResourceType.Gold, 1f } };

    public void Initialize(Vector2Int position)
    {
        this.GridPosition = position;

        Managers.Board.AddBuilding(this);
    }

    protected override void Setup()
    {
        base.Setup();
    }

    protected override void Die()
    {
        Managers.Board.Buildings.Remove(GridPosition);
        base.Die();
    }

    public virtual void TriggerParticleCollision(GameObject collidedWith) { }
}
