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

        if (RaycastAndInteract(inputPos, Constants.Layers.Characters))
        {
            return;
        }

        RaycastAndInteract(inputPos, Constants.Layers.Hexagons);
    }

    private bool RaycastAndInteract(Vector3? inputPos, int layer)
    {
        Ray ray = Managers.Camera.ScreenPointToRay(inputPos.Value);
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            100f,
            layer,
            QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits)
        {
            Interactable interactable;
            if (InterfaceUtility.TryGetInterface<Interactable>(out interactable, hit.collider.gameObject))
            {
                if (interactable.Interact())
                {
                    return true;
                }
            }

            if (hit.transform.parent != null && hit.transform.parent.TryGetComponent<Interactable>(out interactable))
            {
                if (interactable.Interact())
                {
                    return true;
                }
            }
        }

        return false;
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
