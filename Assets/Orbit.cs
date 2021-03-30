using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    private Transform center;
    private float rotationSpeed;
    private float bobDistance;
    private float radius;
    private float startYPosition;
    private float bobOffset;
    private bool hasSetup;

    public void Setup(Vector3 angularVelocity, float rotationSpeed, float bobDistance, Transform center, float startRotationOffset = 0)
    {
        this.bobDistance = bobDistance;
        this.rotationSpeed = rotationSpeed;
        this.center = center;
        this.hasSetup = true;

        if (center != null)
        {
            radius = (center.position - this.transform.position).magnitude;
            transform.RotateAround(center.position, axis, startRotationOffset + rotationSpeed * Time.time);
        }

        this.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
        this.startYPosition = this.transform.position.y;
        bobOffset = Random.Range(0f, 1f);
    }

    void Update()
    {
        if (hasSetup == false)
        {
            return;
        }

        if (center != null)
        {
            transform.RotateAround(center.position, axis, rotationSpeed * Time.deltaTime);
        }

        Vector3 newPos = this.transform.position;
        newPos.y = startYPosition + Mathf.PingPong(Time.time / 20 + bobOffset, bobDistance) - bobDistance / 2;
        this.transform.position = newPos;
    }
}
