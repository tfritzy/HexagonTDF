using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool ContinuousCheck;

    void Update()
    {
        Helpers.FindHexByRaycast()?.Interact();
    }
}
