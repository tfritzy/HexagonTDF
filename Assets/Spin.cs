using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed;
    public Vector3 axis;

    void Update()
    {
        this.transform.Rotate(axis, speed * Time.deltaTime);
    }
}
