using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceNumber : OnScreenNumber
{
    protected override Vector3 InitialVelocity => Vector3.up * 200f;
    protected override float GravityForce => 100f;

    public void SetValue(int value, GameObject owner, ResourceType resourceType)
    {
        SetValue(value, owner);
        this.transform.Find("Icon").GetComponent<Image>().sprite = Prefabs.ResourceIcons[resourceType];
        this.transform.Find("Text").GetComponent<Text>().color = Constants.ResourceColors[resourceType];
    }
}
