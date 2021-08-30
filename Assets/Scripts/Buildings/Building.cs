﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Character
{
    public virtual bool IsWalkable => false;
    public Sprite Icon { get => Prefabs.BuildingIcons[Type]; }
    public ResourceTransaction BuildCost => new ResourceTransaction(this.Power, costRatio);
    public abstract BuildingType Type { get; }
    private Transform deathAnimation;
    private static Dictionary<ResourceType, float> costRatio = new Dictionary<ResourceType, float> { { ResourceType.Gold, 1f } };

    protected override Character FindTargetCharacter()
    {
        return null;
    }

    public void Initialize(Vector2Int position)
    {
        this.GridPosition = position;
        this.deathAnimation = this.transform.Find("Destruction");
        if (this.deathAnimation != null)
        {
            Helpers.TriggerAllParticleSystems(this.deathAnimation, false);
            this.deathAnimation?.gameObject.SetActive(false);
        }

        Managers.Board.AddBuilding(this);
    }

    protected override void Setup()
    {
        base.Setup();
    }

    protected override void Die()
    {
        Managers.Board.Buildings.Remove(GridPosition);
        if (this.deathAnimation != null)
        {
            this.deathAnimation.gameObject.SetActive(true);
            this.deathAnimation.transform.parent = null;
            Helpers.TriggerAllParticleSystems(this.deathAnimation, true);
            this.deathAnimation.Find("Parts").GetComponent<Explodinate>().Explode();
        }

        base.Die();
    }

    public virtual void TriggerParticleCollision(GameObject collidedWith) { }
}
