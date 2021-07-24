using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public bool DisabledByScroll;
    public bool IsFingerHeldDown;

    void Update()
    {
        if (DisabledByScroll == false)
        {
            ShootRayCast();
        }

        if (Managers.CameraControl.IsDragging())
        {
            DisabledByScroll = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisabledByScroll = false;
        }
    }

    private void ShootRayCast()
    {
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

        if (IsPointerOverUIObject())
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

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
