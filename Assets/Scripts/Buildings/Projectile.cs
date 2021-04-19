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
        Destroy(this.gameObject, 30f);
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
            Transform body = other.transform.Find("Body");
            Character character = other.GetComponent<Character>();
            dealDamageToEnemy(attacker, other.GetComponent<Character>(), this.gameObject);
            if (character != null && character.Health <= 0)
            {
                AddForces(body);
            }
            Helpers.TriggerAllParticleSystems(this.explosionParticles, true);
            DetachParticles(this.explosionParticles);
            DetachParticles(this.trailParticles);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void AddForces(Transform body)
    {
        foreach (Rigidbody rb in body.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(this.Rigidbody.velocity / UnityEngine.Random.Range(2, 4) + UnityEngine.Random.insideUnitSphere, ForceMode.VelocityChange);
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