using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 1.5f;              // Speed at which the fish moves
    public float avoidanceStrength = 1.5f;  // Strength of avoidance push from obstacles

    private Vector3 swimDirection;          // Direction the fish swims in
    private bool directionInitialized = false; // Flag to check if direction is set

    // Movement boundaries
    private float leftBound = -16f;
    private float rightBound = 16f;
    private float bottomBound = -7f;
    private float topBound = 7f;

    // Initialize the fish's swimming direction (called by FishSpawner)
    public void InitializeDirection(Vector3 direction)
    {
        swimDirection = direction.normalized;
        directionInitialized = true;
        transform.rotation = Quaternion.LookRotation(swimDirection, Vector3.up);
    }

    void Update()
    {
        if (!directionInitialized) return;

        // Move the fish in the swim direction
        Vector3 movement = swimDirection * speed * Time.deltaTime;
        transform.position += movement;

        // Update fish rotation to face movement direction
        if (swimDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(swimDirection);
        }

        // Check boundaries
        Vector3 pos = transform.position;
        if (pos.x < leftBound || pos.x > rightBound)
        {
            // Deactivate this fish and request a new one
            gameObject.SetActive(false);
            FishSpawner.Instance.ActivateFishFromPool();
        }
        if (pos.z < bottomBound || pos.z > topBound)
        {
            swimDirection.z *= -1; // Reverse vertical direction
        }
    }

    // Adjust swim direction to avoid obstacles (called by FishCollider)
    public void AdjustDirection(Vector3 avoidanceDirection)
    {
        swimDirection += avoidanceDirection * (avoidanceStrength * 1.5f) * Time.deltaTime;
        swimDirection.x = Mathf.Sign(swimDirection.x); // Preserve left/right direction
        swimDirection = new Vector3(swimDirection.x, 0f, swimDirection.z).normalized;
    }

    // Optional: Visualize fish direction in the editor
    void OnDrawGizmos()
    {
        if (directionInitialized)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * 2f); // Local X
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 2f);    // Local Y
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f); // Local Z
        }
    }
}