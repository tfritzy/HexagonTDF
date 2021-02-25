using UnityEngine;

public static class Helpers
{
    public static Hexagon FindHexByRaycast()
    {
        Ray ray = Managers.Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, Constants.Layers.Hexagons))
        {
            return hit.collider.transform?.parent?.GetComponent<Hexagon>();
        }

        return null;
    }
}