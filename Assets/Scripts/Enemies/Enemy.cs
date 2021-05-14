﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Character
{
    public int PathProgress;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => startingHealth;
    public abstract EnemyType Type { get; }
    public abstract Dictionary<AttributeType, float> PowerToAttributeRatio { get; }
    public float MovementSpeedModification;
    public float MovementSpeed;
    public override float Power { get { return power; } }
    private bool isDead;
    private Rigidbody rb;
    private Portal portal;
    private Guid pathId;
    private List<Vector2Int> path;
    private GameObject DeathAnimation;
    private Healthbar healthbar;
    private float baseMovementSpeed;
    private int startingHealth;
    private float power;

    public void SetPortal(Portal portal)
    {
        this.portal = portal;
        this.pathId = portal.PathId;
        this.path = portal.PathToSource;
    }

    public void SetPower(float power, float healthModifier)
    {
        this.power = power;
        this.startingHealth = (int)((this.power * PowerToAttributeRatio[AttributeType.Health]) * Constants.ENEMY_HEALTH_PER_POWER * healthModifier);
        if (startingHealth == 0)
        {
            throw new Exception("Tried to spawn an enemy with 0 health");
        }
        // this.Body.transform.localScale *= healthModifier;
        float movementSpeedPower = PowerToAttributeRatio.ContainsKey(AttributeType.MovementSpeed) ? PowerToAttributeRatio[AttributeType.MovementSpeed] : 0f;
        this.baseMovementSpeed = Constants.ENEMY_DEFAULT_MOVEMENTSPEED * (1 + movementSpeedPower);
    }

    protected override void Setup()
    {
        base.Setup();
        this.PathProgress = 0;
        this.rb = GetComponent<Rigidbody>();
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = baseMovementSpeed;
        this.healthbar = Instantiate(Prefabs.Healthbar,
            new Vector3(10000, 10000),
            new Quaternion(),
            Managers.Canvas).GetComponent<Healthbar>();
        this.healthbar.SetOwner(this.transform);
        this.healthbar.enabled = false;
        SetRagdollState(false);
    }

    protected override void UpdateLoop()
    {
        if (isDead)
        {
            return;
        }

        FollowPath();
        base.UpdateLoop();
    }

    protected override void Die()
    {
        base.Die();

        if (this.DeathAnimation != null)
        {
            this.DeathAnimation.transform.parent = null;
            Destroy(this.DeathAnimation.gameObject, 10f);

            foreach (ParticleSystem ps in this.DeathAnimation.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }

        int goldReward = RollGoldReward();
        Managers.ResourceStore.Add(ResourceType.Gold, goldReward);

        if (goldReward > 0)
        {
            ResourceNumber resourceNumber = Instantiate(Prefabs.ResourceNumber, Managers.Canvas).GetComponent<ResourceNumber>();
            resourceNumber.SetValue(goldReward, this.gameObject, ResourceType.Gold);
        }

        SetRagdollState(true);
        DetachBody();
    }

    private void FollowPath()
    {
        if (portal.PathId != this.pathId)
        {
            RecalculatePath();
        }

        if (PathProgress >= path.Count)
        {
            Managers.Map.Source.TakeDamage(1);
            Destroy(this.gameObject);
            isDead = true;
            return;
        }

        Vector3 difference = (Hexagon.ToWorldPosition(path[PathProgress]) - this.transform.position);
        difference.y = 0;
        this.rb.velocity = difference.normalized * (MovementSpeed + MovementSpeedModification);
        this.transform.rotation = Quaternion.LookRotation(this.rb.velocity, Vector3.up);

        if (difference.magnitude < .1f)
        {
            PathProgress += 1;
        }
    }

    private void RecalculatePath()
    {
        List<Vector2Int> pathToSource = Helpers.FindPath(Managers.Map.Hexagons, Managers.Map.GetBuildingTypeMap(), this.path[PathProgress], Managers.Map.Source.Position);
        this.PathProgress = 0;
        this.pathId = portal.PathId;
        this.path = pathToSource;
    }

    public int RollGoldReward()
    {
        double fullVal = ((float)Power) / (Constants.ResourcePowerMap[ResourceType.Gold] / 4); // Divide by 4 so player can build more stuff.
        double modulous = (int)fullVal > 0 ? (int)fullVal : 1;
        double randomPart = fullVal % modulous;
        return ((int)fullVal) + (UnityEngine.Random.Range(0f, 1f) <= randomPart ? 1 : 0);
    }

    public override void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        this.healthbar.enabled = true;
        base.TakeDamage(amount);
        this.healthbar.SetFillScale((float)this.Health / (float)this.StartingHealth);
    }

    private void SetRagdollState(bool value)
    {
        foreach (Collider collider in this.Body.GetComponentsInChildren<Collider>())
        {
            collider.enabled = value;
        }

        foreach (Rigidbody rb in this.Body.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !value;
        }
    }

    private void DetachBody()
    {
        this.Body.transform.parent = null;
        Destroy(this.Body.gameObject, 5f);
    }
}
