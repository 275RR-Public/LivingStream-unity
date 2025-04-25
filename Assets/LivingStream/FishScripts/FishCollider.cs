using UnityEngine;

public class FishCollider : MonoBehaviour
{
    private FishMovement fishMovement;

    void Start()
    {
        // Find the FishMovement script on the parent
        fishMovement = GetComponentInParent<FishMovement>();
        if (fishMovement == null)
        {
            Debug.LogError("FishMovement script not found on parent!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (fishMovement == null) return;

        // Check if the collider is a rock or another fish
        // add:   || other.CompareTag("Fish")   if fish should avoid fish
        if (other.CompareTag("Rocks") || other.CompareTag("TrackedPerson"))
        {
            // Calculate the closest point on the other collider
            Vector3 closestPoint = other.ClosestPoint(transform.position);
            // Direction away from the obstacle
            Vector3 away = transform.position - closestPoint;
            away.y = 0f; // Keep movement horizontal
            // Adjust fish direction to avoid the obstacle
            fishMovement.AdjustDirection(away.normalized);
        }
    }
}