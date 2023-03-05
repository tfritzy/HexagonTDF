using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    private Transform mainCharacter;
    private Vector3 Offset = new Vector3(0, 13, -10);

    void Start()
    {
        this.mainCharacter = Managers.MainCharacter.transform;
    }


    void Update()
    {
        this.transform.position = this.mainCharacter.transform.position + Offset;
    }
}