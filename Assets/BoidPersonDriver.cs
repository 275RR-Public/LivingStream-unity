using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class BoidPersonDriver : MonoBehaviour
{
    [Tooltip("The VFX component running VFXG_FakeBoids_Fish")]
    public VisualEffect boidVFX;

    [Tooltip("Your ObjectTracking script instance")]
    public ObjectTracking tracker;

    [Tooltip("How big the collision sphere should be")]
    public float personRadius = 2f;

    // Must match exactly the names you exposed in the blackboard:
    const string k_PersonPos   = "PersonPosition";
    const string k_PersonRadius = "PersonRadius";

    void Update()
    {
        if (boidVFX == null || tracker == null)
            return;

        // Pick one tracked cube (here: the first one in the dictionary).
        var first = tracker.TrackedObjects.Values.FirstOrDefault();
        if (first != null)
        {
            Vector3 pos = first.transform.position;
            boidVFX.SetVector3(k_PersonPos, pos);
            boidVFX.SetFloat   (k_PersonRadius, personRadius);
        }
        else
        {
            // No one tracked: push the sphere far away so it never collides.
            boidVFX.SetVector3(k_PersonPos, new Vector3(9999,9999,9999));
        }
    }
}
