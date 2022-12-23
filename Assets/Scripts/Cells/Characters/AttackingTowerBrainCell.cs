using UnityEngine;

public class AttackTowerBrainCell : BrainCell
{
    public override void Update()
    {
        this.Target = FindTarget();
        Attack();
    }

    private void Attack()
    {
        if (this.Target != null && this.Owner.AttackCell.CanAttack())
        {
            this.Owner.AttackCell.ReleaseAttack(this.Target);
        }
    }

    private float lastTargetCheckTime;
    private Character FindTarget()
    {
        if (Time.time < lastTargetCheckTime + 1f)
        {
            return this.Target;
        }
        lastTargetCheckTime = Time.time;

        foreach (Collider hit in Physics.OverlapSphere(this.Owner.transform.position, this.Owner.AttackCell.Range, Constants.Layers.Units))
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit.Alliance == this.Owner.Enemies)
            {
                return unit;
            }
        }

        return null;
    }
}