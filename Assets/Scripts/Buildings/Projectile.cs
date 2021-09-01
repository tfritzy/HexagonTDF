using System;
using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public delegate void DealDamageToEnemy(Character attacker, Character target);
    private DealDamageToEnemy dealDamageToEnemy;
    public delegate bool IsCollisionTarget(Character attacker, GameObject collision);
    private IsCollisionTarget isCollisionTarget;
    protected Rigidbody Rigidbody;
    protected float birthTime;
    private Character attacker;
    private Transform trailParticles;
    private Transform explosionParticles;
    private bool upForceOnDeath;
    private bool isTracking;
    private bool isInitialized;
    private bool canHitMultipleTargets;
    private HashSet<GameObject> hits;

    void Start()
    {
        this.trailParticles = this.transform.Find("Trail");
        this.explosionParticles = this.transform.Find("Explosion");
        Helpers.TriggerAllParticleSystems(this.explosionParticles, false);
        hits = new HashSet<GameObject>();
        StartLogic();
        Destroy(this.gameObject, 30f);
    }

    void Update()
    {
        UpdateLoop();
    }

    public void Initialize(
        DealDamageToEnemy damageEnemyHandler,
        IsCollisionTarget isCollisionTarget,
        Character attacker,
        bool upForceOnDeath = false,
        bool canHitMultipleTargets = false)
    {
        this.dealDamageToEnemy = damageEnemyHandler;
        this.isCollisionTarget = isCollisionTarget;
        this.attacker = attacker;
        this.birthTime = Time.time;
        this.upForceOnDeath = upForceOnDeath;
        this.canHitMultipleTargets = canHitMultipleTargets;
        SetupRigidbody();
        this.isInitialized = true;
    }

    private GameObject trackingTarget;
    private float trackingMovementSpeed;
    public void SetTracking(GameObject target, float movementSpeed)
    {
        isTracking = true;
        this.trackingTarget = target;
        this.trackingMovementSpeed = movementSpeed;
    }

    protected virtual void UpdateLoop()
    {
        if (this.isTracking)
        {
            if (this.trackingTarget == null)
            {
                ExplodeAnimation();
            }

            this.Rigidbody.velocity = (this.trackingTarget.transform.position - this.transform.position).normalized * this.trackingMovementSpeed;
        }
    }
    protected virtual void StartLogic() { }

    private void OnTriggerEnter(Collider other)
    {
        if (this.isInitialized == false)
        {
            return;
        }

        if (other?.gameObject == null)
        {
            return;
        }

        if (hits.Count > 0 && !canHitMultipleTargets)
        {
            return;
        }

        if (isCollisionTarget(attacker, other.gameObject))
        {
            if (hits.Contains(other.gameObject))
            {
                return;
            }

            hits.Add(other.gameObject);
            OnCollision(other.gameObject);
            Character character = other.GetComponent<Character>();
            dealDamageToEnemy(attacker, other.GetComponent<Character>());
            ExplodeAnimation();
        }
    }

    private void ExplodeAnimation()
    {
        Helpers.TriggerAllParticleSystems(this.explosionParticles, true);
        DetachParticles(this.explosionParticles);
        DetachParticles(this.trailParticles);

        if (!canHitMultipleTargets)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void SetupRigidbody()
    {
        this.Rigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.Rigidbody.useGravity = false;
    }

    protected virtual void OnCollision(GameObject other) { }

    private void DetachParticles(Transform particleGroup)
    {
        if (particleGroup == null)
        {
            return;
        }

        particleGroup.parent = null;
        GameObject.Destroy(particleGroup.gameObject, 5f);
    }
}