using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Character
{
    public int PathProgress;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public abstract float BaseMovementSpeed { get; }
    public abstract EnemyType Type { get; }

    public float MovementSpeedModification;
    public float MovementSpeed;
    private bool isDead;
    private Rigidbody rb;
    private Portal portal;
    private Guid pathId;
    private List<Vector2Int> path;
    private GameObject DeathAnimation;
    private Healthbar healthbar;

    public override float Power
    {
        get { return ((float)StartingHealth / 2.5f) + (BaseMovementSpeed - 1); }
    }

    public void SetPortal(Portal portal)
    {
        this.portal = portal;
        this.pathId = portal.PathId;
        this.path = portal.PathToSource;
    }

    protected override void Setup()
    {
        this.PathProgress = 0;
        this.rb = GetComponent<Rigidbody>();
        this.DeathAnimation = transform.Find("DeathAnimation")?.gameObject;
        this.MovementSpeed = BaseMovementSpeed;
        this.healthbar = Instantiate(Prefabs.Healthbar,
            new Vector3(10000, 10000),
            new Quaternion(),
            Managers.Canvas).GetComponent<Healthbar>();
        this.healthbar.SetOwner(this.transform);
        this.healthbar.enabled = false;
        base.Setup();
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

        Managers.ResourceStore.Add(ResourceType.Gold, GoldReward);
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

    public int GoldReward
    {
        get
        {
            double fullVal = ((float)Power) / 20f;
            double modulous = (int)fullVal > 0 ? (int)fullVal : 1;
            double randomPart = fullVal % modulous;
            return ((int)fullVal) + (UnityEngine.Random.Range(0f, 1f) <= randomPart ? 1 : 0);
        }
    }

    public override void TakeDamage(int amount)
    {
        this.healthbar.enabled = true;
        base.TakeDamage(amount);
        this.healthbar.SetFillScale((float)this.Health / (float)this.StartingHealth);
    }
}
