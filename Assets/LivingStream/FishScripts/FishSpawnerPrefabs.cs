using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public static FishSpawner Instance;
    public GameObject[] fishPrefabs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnInitialFish(10);
    }

    public void SpawnNewFish(Vector3 position, Vector3 swimDirection)
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            Debug.LogError("❌ No fish prefabs assigned in FishSpawner!");
            return;
        }

        GameObject selectedPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        position.y = selectedPrefab.transform.position.y;

        GameObject newFish = Instantiate(selectedPrefab, position, Quaternion.identity);
        FishMovement fishMovement = newFish.GetComponent<FishMovement>();

        if (fishMovement != null)
        {
            fishMovement.InitializeDirection(swimDirection);
            fishMovement.StartCoroutine(fishMovement.FadeIn());
        }
        else
        {
            Debug.LogError("❌ FishMovement script not found on new fish!");
        }

        Debug.Log($"New fish spawned at {position} swimming {swimDirection}");
    }

    public void SpawnInitialFish(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-14f, 14f);
            float z = Random.Range(-7f, 7f);
            GameObject selectedPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
            float y = selectedPrefab.transform.position.y;

            Vector3 spawnPos = new Vector3(x, y, z);
            Vector3 randomDir = Random.value < 0.5f ? Vector3.left : Vector3.right;
            SpawnNewFish(spawnPos, randomDir);
        }
    }
}
