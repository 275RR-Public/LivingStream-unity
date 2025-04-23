/* ##### SPAWN 10 RANDOM FISH FROM START ##### */
/* ##### FISH SPAWN AT Y POSITION OF PREFAB ##### */
/* ##### FISH RESPAWN AT BORDER & SWIM CORRECTLY (no continual prefab switching at border) ##### */
/* ##### FISH SWAY AWAY FROM EACH OTHER & ROCKS IN ENVIRONMENT ##### */
/* ##### RUN WITH FS5.cs ##### */

using UnityEngine;
using System.Collections;

public class FishMovement : MonoBehaviour
{
    public float speed = 1.5f;
    public float fadeDuration = 1f;

    private Vector3 swimDirection;
    private Renderer fishRenderer;
    private bool isFading = false;
    private bool directionInitialized = false;

    private float leftBound = -16f;
    private float rightBound = 16f;
    private float bottomBound = -7f;
    private float topBound = 7f;

    public float avoidanceStrength = 1.5f;  // How hard to push away


    // Set externally by FishSpawner
    public void InitializeDirection(Vector3 direction)
    {
        swimDirection = direction.normalized;
        directionInitialized = true;

        string prefabName = gameObject.name.Replace("(Clone)", "").Trim();

        float yRotationOverride = prefabName switch
        {
            "Spotted Gar No Animation" => 90f,
            "Blue Catfish Finished" => 90f,
            "BlackCrappieNoAnimation" => 180f,
            _ => -1f // Use default logic
        };

        if (yRotationOverride >= 0f)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                yRotationOverride,
                transform.rotation.eulerAngles.z
            );
        }
        else if (swimDirection == Vector3.left)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + 180f,
                transform.rotation.eulerAngles.z
            );
        }
    }

    void Start()
    {
        fishRenderer = GetComponentInChildren<Renderer>();

        if (!directionInitialized)
        {
            InitializeDirection(Random.value < 0.5f ? Vector3.left : Vector3.right);
        }
    }

    void Update()
    {
        if (isFading) return;

        Vector3 movement = new Vector3(swimDirection.x, 0f, swimDirection.z);
        transform.position += movement * speed * Time.deltaTime;

        Vector3 pos = transform.position;

        if (pos.x < leftBound || pos.x > rightBound)
        {
            StartCoroutine(FadeOutAndRespawn());
        }

        if (pos.z < bottomBound || pos.z > topBound)
        {
            swimDirection.z *= -1;
        }
    }

    IEnumerator FadeOutAndRespawn()
    {
        isFading = true;
        float t = 0;
        Material material = fishRenderer.material;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            Color color = material.color;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            material.color = color;
            yield return null;
        }

        if (FishSpawner.Instance != null)
        {
            Vector3 newDirection = swimDirection == Vector3.left ? Vector3.right : Vector3.left;
            Vector3 safePos = GetSafeRespawnPosition();
            FishSpawner.Instance.SpawnNewFish(safePos, newDirection);
        }
        else
        {
            Debug.LogError("❌ FishSpawner.Instance is NULL!");
        }

        Destroy(gameObject);
    }

    Vector3 GetSafeRespawnPosition()
    {
        float x = transform.position.x;

        if (x <= leftBound)
            x = leftBound + 1f;
        else if (x >= rightBound)
            x = rightBound - 1f;

        return new Vector3(x, transform.position.y, transform.position.z);
    }

    public IEnumerator FadeIn()
    {
        float t = 0;
        Material material = GetComponentInChildren<Renderer>().material;
        Color startColor = material.color;
        Color color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        material.color = color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            color.a = alpha;
            material.color = color;
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Avoid other fish
        if (other.CompareTag("Fish") && other.gameObject != this.gameObject)
        {
            Vector3 away = transform.position - other.transform.position;
            away.y = 0f;

            swimDirection += away.normalized * avoidanceStrength * Time.deltaTime;
            swimDirection.x = Mathf.Sign(swimDirection.x);
            swimDirection = new Vector3(swimDirection.x, 0f, swimDirection.z).normalized;
        }

        // Avoid rocks or environmental obstacles
        if (other.CompareTag("Rocks"))
        {
            Vector3 away = transform.position - other.ClosestPoint(transform.position);
            away.y = 0f;

            swimDirection += away.normalized * (avoidanceStrength * 1.5f) * Time.deltaTime;
            swimDirection.x = Mathf.Sign(swimDirection.x);
            swimDirection = new Vector3(swimDirection.x, 0f, swimDirection.z).normalized;
        }
    }
}
