using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    private MeshRenderer mr;
    public int direction = 1;

    void Start()
    {
        this.mr = this.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Vector2 offset = this.mr.material.mainTextureOffset;
        offset.x += direction * .2f * Time.deltaTime;
        this.mr.material.mainTextureOffset = offset;
    }
}
