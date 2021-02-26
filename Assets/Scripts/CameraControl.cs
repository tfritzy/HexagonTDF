using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float INPUT_BUFFER_DURATION = .25f;
    private const float MAX_CAMERA_VELOCITY = 20f;

    public const float MOVEMENT_SPEED = 9f;
    private Vector3? touchStartPosition;
    private Vector3 originalCameraPosition;
    private Vector3 lastInputPosition;
    private float pixelToWorldConversion;
    private float touchStartTime;
    private Rigidbody rb;
    private LinkedList<InputBufferEntry> previousInputBuffer;

    struct InputBufferEntry
    {
        public Vector3 Position;
        public float Time;
    }

    void Start()
    {
        pixelToWorldConversion = Managers.Camera.orthographicSize / (Screen.height / 3);
        rb = this.GetComponent<Rigidbody>();
        previousInputBuffer = new LinkedList<InputBufferEntry>();
    }

    void Update()
    {
        HandleTouchInput();
        this.transform.position += GetDirectionalInput() * MOVEMENT_SPEED * Time.deltaTime;
    }

    private void HandleTouchInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (touchStartPosition.HasValue == false)
            {
                touchStartPosition = Input.mousePosition;
                originalCameraPosition = Managers.Camera.transform.position;
                touchStartTime = Time.time;
            }

            ManagePreviousInputBuffer(Input.mousePosition, previousInputBuffer);
            rb.velocity = Vector3.zero;
            Vector3 delta = GetScrollVector(Input.mousePosition, touchStartPosition.Value);
            this.transform.position = originalCameraPosition + delta;
            lastInputPosition = Input.mousePosition;
        }
        else
        {
            if (touchStartPosition.HasValue)
            {
                this.rb.velocity = GetTouchEndVelocity(previousInputBuffer);
            }

            touchStartPosition = null;
            previousInputBuffer = new LinkedList<InputBufferEntry>();
        }
    }

    private void ManagePreviousInputBuffer(Vector3 nextInputPosition, LinkedList<InputBufferEntry> previousInputBuffer)
    {
        InputBufferEntry newEntry = new InputBufferEntry()
        {
            Position = nextInputPosition,
            Time = Time.time,
        };

        previousInputBuffer.AddLast(newEntry);

        while (previousInputBuffer.Count > 0 && Time.time - previousInputBuffer.First.Value.Time > INPUT_BUFFER_DURATION)
        {
            previousInputBuffer.RemoveFirst();
        }
    }

    private Vector3 GetTouchEndVelocity(LinkedList<InputBufferEntry> previousInputBuffer)
    {
        float timeDelta = previousInputBuffer.Last.Value.Time - previousInputBuffer.First.Value.Time;
        Vector3 delta = GetScrollVector(previousInputBuffer.Last.Value.Position, previousInputBuffer.First.Value.Position);

        if (delta.magnitude < .2f)
        {
            return Vector3.zero;
        }

        Vector3 velocity = delta / timeDelta;

        if (velocity.magnitude > MAX_CAMERA_VELOCITY)
        {
            velocity = velocity.normalized * MAX_CAMERA_VELOCITY;
        }

        return velocity;
    }

    private Vector3 GetScrollVector(Vector3 inputScreenPosition, Vector3 startScreenPosition)
    {
        Vector3 delta = Input.mousePosition - startScreenPosition;
        delta.z = delta.y;
        delta.y = 0;
        return -delta * pixelToWorldConversion;
    }

    public Vector3 GetDirectionalInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }

        return direction.normalized;
    }
}
