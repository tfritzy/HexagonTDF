using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : OnScreenNumber
{
    protected override Vector3 InitialVelocity => Vector3.up * 200f + (Vector3)Random.insideUnitCircle * 100f;
    protected override float GravityForce => 500f;
}
