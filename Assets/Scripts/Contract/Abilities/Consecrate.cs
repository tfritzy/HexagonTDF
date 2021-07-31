using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consecrate : HexagonTargetAbility
{
    public override float Cooldown => 30;
    public const float RANGE = 3f;
    public const int DAMAGE = 200;

    public Consecrate(Hero owner) : base(owner) { }

    protected override void Execute()
    {
        GameObject explosion = GameObject.Instantiate(
            Managers.Prefabs.ConsecrateEffect,
            this.Target.transform.position,
            new Quaternion());
        GameObject.Destroy(explosion, 10f);

        Collider[] hits = Physics.OverlapSphere(
            this.Target.transform.position,
            RANGE,
            Constants.Layers.Characters,
            QueryTriggerInteraction.Collide);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Character>(out Character character))
            {
                if (character.Alliance == this.Owner.Enemies)
                {
                    character.TakeDamage(DAMAGE, Owner, 1);
                }
            }
        }
    }
}
