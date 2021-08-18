using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodinate : MonoBehaviour
{
    private Collider[] parts;

    void Start()
    {
        parts = this.GetComponentsInChildren<Collider>();

        foreach (Collider part in parts)
        {
            Rigidbody rigidbody = part.GetComponent<Rigidbody>();
            rigidbody.velocity = new Vector3(Random.Range(-3, 3), Random.Range(3, 6), Random.Range(-3, 3));
            rigidbody.useGravity = true;
            rigidbody.angularVelocity = Random.insideUnitSphere * Random.Range(10, 30);
        }
    }
}
