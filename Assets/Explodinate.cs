using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodinate : MonoBehaviour
{
    private Rigidbody[] parts;

    void Start()
    {

    }

    public void Explode()
    {
        parts = this.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in parts)
        {
            rb.velocity = new Vector3(Random.Range(-3, 3), Random.Range(3, 6), Random.Range(-3, 3));
            rb.useGravity = true;
            rb.angularVelocity = Random.insideUnitSphere * Random.Range(10, 30);
        }
    }
}
