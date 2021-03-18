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

    void Start()
    {
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

        GameObject.Destroy(this.gameObject, 10f);
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
            dealDamageToEnemy(attacker, other.transform.GetComponent<Character>(), this.gameObject);
            GameObject.Destroy(this.gameObject);
        }
    }

    private bool isCollisionTarget(Character attacker, GameObject other)
    {
        if (other.TryGetComponent<Character>(out Character targetCharacter))
        {
            return attacker.Enemies == targetCharacter.Alliance;
        }

        return false;
    }

    private void SetupRigidbody()
    {
        this.Rigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.Rigidbody.useGravity = false;
    }
}