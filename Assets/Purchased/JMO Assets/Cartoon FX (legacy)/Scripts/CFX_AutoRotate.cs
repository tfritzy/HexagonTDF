using UnityEngine;

public class CFX_AutoRotate : MonoBehaviour
{
    public Vector3 MinRotation;
    public Vector3 MaxRotation;
    private Vector3 rotation;

    void Start()
    {
        rotation = new Vector3(
            Random.Range(MinRotation.x, MaxRotation.x),
            Random.Range(MinRotation.y, MaxRotation.y),
            Random.Range(MinRotation.z, MaxRotation.z));
    }

    void Update()
    {
        this.transform.Rotate(rotation * Time.deltaTime);
    }
}
