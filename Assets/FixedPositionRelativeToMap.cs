using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPositionRelativeToMap : MonoBehaviour
{
    private Vector3 rootPosition;

    public void SetTether(GameObject tether)
    {
        this.rootPosition = tether.transform.position;
    }

    void Update()
    {
        this.transform.position = Managers.Camera.WorldToScreenPoint(rootPosition);
    }
}
