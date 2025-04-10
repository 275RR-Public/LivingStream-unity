using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteInEditMode]
public class FloatingIceberg : MonoBehaviour
{
    public WaterSurface targetSurface = null;
    public float selfRotationSpeed = 0; // Optional: keep if rotation is desired
    public float initialScale = 0.25f;  // Optional: for initial sizing
    public bool includeDeformers = true;

    // Internal search params
    private WaterSearchParameters searchParameters = new WaterSearchParameters();
    private WaterSearchResult searchResult = new WaterSearchResult();

    void Start()
    {
        // Set initial scale only (position is handled by ObjectTracking)
        this.transform.localScale = Vector3.one * initialScale;
    }

    void Update()
    {
        // Optional: Rotate the iceberg if desired
        if (selfRotationSpeed > 0)
        {
            Vector3 r = this.transform.localEulerAngles;
            r.y += selfRotationSpeed;
            this.transform.localEulerAngles = r;
        }

        if (targetSurface != null)
        {
            // Build the search parameters
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = this.transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;
            searchParameters.includeDeformation = includeDeformers;
            searchParameters.excludeSimulation = false;

            // Project the current position onto the water surface and adjust y only
            if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
            {
                Vector3 newPosition = this.transform.position;
                newPosition.y = searchResult.projectedPositionWS.y;
                this.transform.position = newPosition;
            }
            // Else: If projection fails, keep current position (no action needed)
        }
    }
}