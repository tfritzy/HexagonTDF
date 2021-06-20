using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void DealDamageToEnemy(Character attacker, Character target, GameObject projectile);
    private DealDamageToEnemy dealDamageToEnemy;
    public delegate bool IsCollisionTarget(Character attacker, GameObject collision);
    private IsCollisionTarget isCollisionTarget;
    protected Rigidbody Rigidbody;
    protected float birthTime;
    private Character attacker;
    private bool hasAlreadyTriggered;
    private Transform trailParticles;
    private Transform explosionParticles;
    private bool upForceOnDeath;
    private bool isTracking;

    void Start()
    {
        this.trailParticles = this.transform.Find("Trail");
        this.explosionParticles = this.transform.Find("Explosion");
        Helpers.TriggerAllParticleSystems(this.explosionParticles, false);
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
        bool upForceOnDeath = false)
    {
        this.dealDamageToEnemy = damageEnemyHandler;
        this.isCollisionTarget = isCollisionTarget;
        this.attacker = attacker;
        this.birthTime = Time.time;
        this.upForceOnDeath = upForceOnDeath;
        SetupRigidbody();
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
        if (other?.gameObject == null)
        {
            return;
        }

        if (hasAlreadyTriggered)
        {
            return;
        }

        if (isCollisionTarget(attacker, other.gameObject))
        {
            hasAlreadyTriggered = true;
            OnCollision(other.gameObject);
            Character character = other.GetComponent<Character>();
            dealDamageToEnemy(attacker, other.GetComponent<Character>(), this.gameObject);
            ExplodeAnimation();
        }
    }

    private void ExplodeAnimation()
    {
        Helpers.TriggerAllParticleSystems(this.explosionParticles, true);
        DetachParticles(this.explosionParticles);
        DetachParticles(this.trailParticles);
        GameObject.Destroy(this.gameObject);
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