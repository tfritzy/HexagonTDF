using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceNumber : OnScreenNumber
{
    protected override Vector3 InitialVelocity => Vector3.up * 200f;
    protected override float GravityForce => 100f;

    public void SetValue(int value, GameObject owner, ResourceType resourceType, bool isStatic = false)
    {
        SetValue(value, owner);
        this.transform.Find("Text").GetComponent<Text>().color = Constants.ResourceColors[resourceType];
        this.isStatic = isStatic;
    }
}
