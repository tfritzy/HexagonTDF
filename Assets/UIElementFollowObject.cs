using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementFollowObject : MonoBehaviour
{
    public GameObject ObjectToFollow;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (ObjectToFollow == null)
        {
            Destroy(this.gameObject);
        }

        this.transform.position = Managers.Camera.WorldToScreenPoint(ObjectToFollow.transform.position);
    }
}
