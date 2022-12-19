using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        this.transform.RotateAround(this.transform.position, Vector3.up, Random.Range(0, 360));
    }
}
