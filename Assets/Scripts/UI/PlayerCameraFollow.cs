using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    private Transform mainCharacter;
    private Vector3 Offset = new Vector3(0, 13, -10);

    void Update()
    {
        if (this.mainCharacter == null)
        {
            this.mainCharacter = Managers.MainCharacter?.transform;
        }
        else
        {
            this.transform.position = this.mainCharacter.transform.position + Offset;
        }
    }
}