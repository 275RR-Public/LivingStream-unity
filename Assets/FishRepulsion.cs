using UnityEngine;

public class FishRepulsion : MonoBehaviour
{
    [Tooltip("Reference your ObjectTracking component here")]
    public ObjectTracking tracker;

    [Tooltip("Tag used by all fish objects")]
    public string fishTag = "Fish";

    [Tooltip("How close fish must get to repel (horizontal distance)")]
    public float influenceRadius = 3f;

    [Tooltip("Sideways push speed (units/sec)")]
    public float pushSpeed = 3f;

    void Update()
    {
        if (tracker == null) return;

        // Grab all runtime‐spawned fish
        var fishArray = GameObject.FindGameObjectsWithTag(fishTag);
        if (fishArray.Length == 0) return;

        // For each tracked person
        foreach (var kv in tracker.TrackedObjects)
        {
            Vector3 personPos = kv.Value.transform.position;

            // Only use XZ of person, so the "sphere" extends infinitely up/down
            float px = personPos.x;
            float pz = personPos.z;

            // Push every fish away on the XZ plane
            foreach (var fish in fishArray)
            {
                Vector3 fPos = fish.transform.position;
                // horizontal delta
                float dx = fPos.x - px;
                float dz = fPos.z - pz;

                float dist = Mathf.Sqrt(dx * dx + dz * dz);
                if (dist > 0f && dist < influenceRadius)
                {
                    // direction purely in XZ
                    Vector3 dir = new Vector3(dx, 0f, dz).normalized;
                    // apply push
                    fish.transform.position = fPos + dir * pushSpeed * Time.deltaTime;
                    // Y stays exactly what it was (we never touched fPos.y)
                }
            }
        }
    }
}
