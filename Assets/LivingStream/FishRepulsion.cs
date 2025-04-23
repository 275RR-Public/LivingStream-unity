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

        // Cache once per frame
        var fishArray = GameObject.FindGameObjectsWithTag(fishTag);
        if (fishArray.Length == 0) return;

        // For each tracked person
        foreach (var kv in tracker.TrackedObjects)
        {
            Vector3 person = kv.Value.transform.position;
            float px = person.x, pz = person.z;

            // Push all fish away in XZ
            foreach (var fish in fishArray)
            {
                Vector3 fPos = fish.transform.position;
                float dx = fPos.x - px;
                float dz = fPos.z - pz;
                float dist = Mathf.Sqrt(dx * dx + dz * dz);

                if (dist > 0f && dist < influenceRadius)
                {
                    // horizontal direction
                    Vector3 dir = new Vector3(dx, 0f, dz).normalized;
                    // target further out at the edge of your sphere
                    Vector3 target = new Vector3(
                        px + dir.x * influenceRadius,
                        fPos.y,
                        pz + dir.z * influenceRadius
                    );
                    // smoothly move toward that target
                    fish.transform.position = Vector3.MoveTowards(
                        fPos,
                        target,
                        pushSpeed * Time.deltaTime
                    );
                }
            }
        }
    }
}
