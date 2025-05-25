using UnityEngine;

public class CollisionDetectorRaycast : MonoBehaviour
{
    public LayerMask detectionLayers;
    public Vector3 rayDirection = Vector3.forward;
    Camera mainCamera;
    public bool isColliding { get; private set; }
    Vector3 finalRayDirection;
    public float rayLength = 1f;
    public bool rotateWithTransform = false;

    public delegate void CollissionStateChangedAction(bool state);
    public event CollissionStateChangedAction onCollisionStateChanged;

    [HideInInspector] public RaycastHit outHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;       
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateWithTransform)
        {
            finalRayDirection = transform.TransformDirection(rayDirection);
        }
        else if (mainCamera != null)
        {
            // Make direction relative to the camera
            Vector3 camRight = mainCamera.transform.right;
            camRight.y = 0f; // Optional: flatten to horizontal plane
            camRight.Normalize();

            finalRayDirection = rayDirection.x > 0 ? camRight : -camRight;
        }
        else
        {
            finalRayDirection = rayDirection;
        }

        RaycastHit hit;
        bool hitDetected = Physics.Raycast(transform.position, finalRayDirection.normalized * rayLength, out hit, rayLength, detectionLayers);
        outHit = hit;

        if (hitDetected && !isColliding) 
        {
            setCollissionState(true);
        }
        else if (!hitDetected && isColliding)
        {
            setCollissionState(false);
        }

    }

    void setCollissionState(bool state)
    {
        if (isColliding != state)
        { 
            isColliding = state;
            onCollisionStateChanged?.Invoke(isColliding);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, finalRayDirection.normalized * rayLength);
    }
}
