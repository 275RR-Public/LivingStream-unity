using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class BoidPersonDriver : MonoBehaviour
{
    [Tooltip("The VFX component running VFXG_FakeBoids_Fish")]
    public VisualEffect boidVFX;

    [Tooltip("Your ObjectTracking script instance")]
    public ObjectTracking tracker;

    [Tooltip("How big the collision sphere should be")]
    public float personRadius = 2f;

    [Tooltip("Fixed Y height for the collision sphere (e.g. water level)")]
    public float sphereY = 0.5f;

    // Blackboard parameter names
    static readonly int k_PersonPosID    = Shader.PropertyToID("PersonPosition");
    static readonly int k_PersonRadiusID = Shader.PropertyToID("PersonRadius");

    void Reset()
    {
        // Try auto‑assigning the VFX if you forgot in the Inspector
        if (boidVFX == null) boidVFX = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (boidVFX == null || tracker == null)
            return;

        // Take the first tracked person (if any)
        var firstGO = tracker.TrackedObjects.Values.FirstOrDefault();
        if (firstGO != null)
        {
            // Grab its XZ, but force Y to sphereY
            Vector3 raw = firstGO.transform.position;
            Vector3 fixedPos = new Vector3(raw.x, sphereY, raw.z);

            boidVFX.SetVector3(k_PersonPosID, fixedPos);
            boidVFX.SetFloat   (k_PersonRadiusID, personRadius);
        }
        else
        {
            // No one tracked: move it far away
            boidVFX.SetVector3(k_PersonPosID, new Vector3(9999f, sphereY, 9999f));
        }
    }
}
