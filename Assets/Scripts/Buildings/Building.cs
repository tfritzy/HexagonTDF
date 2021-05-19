﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Character
{
    public Sprite Icon { get => Prefabs.BuildingIcons[Type]; }
    public abstract BuildingType Type { get; }
    public Vector2Int GridPosition { get; set; }
    public override int StartingHealth => int.MaxValue;
    public ResourceTransaction BuildCost => new ResourceTransaction(this.Power, costRatio);
    public override VerticalRegion Region => VerticalRegion.Ground;
    public virtual bool IsWalkable => false;
    private static Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float> { { ResourceType.Gold, 1f } };

    public void Initialize(Vector2Int position)
    {
        this.GridPosition = position;

        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = Constants.Materials.Normal;
        }

        Managers.Board.AddBuilding(this);
    }

    protected override void Setup()
    {
        base.Setup();
    }

    public virtual void TriggerParticleCollision(GameObject collidedWith) { }
}
