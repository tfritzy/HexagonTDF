using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        ShootRayCast();
    }

    private void ShootRayCast()
    {
        if (Managers.CameraControl.IsDragging())
        {
            return;
        }

        Vector3? inputPos = null;
        if (Input.GetMouseButtonUp(0))
        {
            inputPos = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            inputPos = Input.GetTouch(0).position;
        }
        if (!inputPos.HasValue)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(inputPos.Value);
        RaycastHit hit;
        if (Physics.Raycast(
            ray,
            out hit,
            100f,
            Constants.Layers.Hexagons | Constants.Layers.Characters,
            QueryTriggerInteraction.Collide))
        {
            if (InterfaceUtility.TryGetInterface<Interactable>(out Interactable interactable, hit.collider.gameObject))
            {
                interactable.Interact();
            }

            if (hit.transform.parent != null && hit.transform.parent.TryGetComponent<HexagonMono>(out HexagonMono hexagon))
            {
                hexagon.Interact();
            }
        }
    }
}
