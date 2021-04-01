using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void DealDamageToEnemy(Character attacker, Character target, GameObject projectile);
    private DealDamageToEnemy dealDamageToEnemy;
    protected Rigidbody Rigidbody;
    protected Character target;
    protected float birthTime;
    private Character attacker;
    private bool hasAlreadyTriggered;
    private Transform trailParticles;
    private Transform explosionParticles;

    void Start()
    {
        this.trailParticles = this.transform.Find("Trail");
        this.explosionParticles = this.transform.Find("Explosion");
        Helpers.TriggerAllParticleSystems(this.explosionParticles, false);
        StartLogic();
    }

    void Update()
    {
        UpdateLoop();
    }

    public void Initialize(
        DealDamageToEnemy damageEnemyHandler,
        Character attacker,
        Character target = null)
    {
        this.dealDamageToEnemy = damageEnemyHandler;
        this.attacker = attacker;
        this.target = target;
        this.birthTime = Time.time;
        SetupRigidbody();
    }

    protected virtual void UpdateLoop() { }
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
            dealDamageToEnemy(attacker, other.transform.GetComponent<Character>(), this.gameObject);
            Helpers.TriggerAllParticleSystems(this.explosionParticles, true);
            DetachParticles(this.explosionParticles);
            DetachParticles(this.trailParticles);
            GameObject.Destroy(this.gameObject);
        }
    }

    private bool isCollisionTarget(Character attacker, GameObject other)
    {
        if (other.TryGetComponent<Character>(out Character targetCharacter))
        {
            return attacker.Enemies == targetCharacter.Alliance;
        }

        if (other.CompareTag(Constants.Tags.Hexagon))
        {
            return true;
        }

        return false;
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