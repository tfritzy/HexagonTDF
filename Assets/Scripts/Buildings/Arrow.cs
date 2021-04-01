using UnityEngine;

public class Arrow : Projectile
{
    protected override void OnCollision(GameObject other)
    {
        base.OnCollision(other);
        Transform body = this.transform.Find("Body");
        body.parent = other.transform;
    }
}