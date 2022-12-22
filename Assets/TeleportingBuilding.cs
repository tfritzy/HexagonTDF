using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingBuilding : MonoBehaviour
{
    public float Bounds = 10f;

    private float lastTeleportTime;

    void Update()
    {
        if (Time.time > lastTeleportTime + 10f)
        {
            this.transform.position = new Vector3(Random.Range(-Bounds, Bounds), 0, Random.Range(-Bounds, Bounds));
            lastTeleportTime = Time.time;
        }
    }
}
