using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FishSpawner : MonoBehaviour
{
    public static FishSpawner Instance;

    // Array to assign multiple fish prefabs in the Inspector
    public GameObject[] fishPrefabs;

    // Number of fish to spawn initially and pool size
    public int initialFishCount = 7;
    public int poolSize = 14; // must be >= number of fish types (currently 5 types)

    // Configurable spawn area boundaries
    public float spawnMinX = -14f;
    public float spawnMaxX = 14f;
    public float spawnMinZ = -7f;
    public float spawnMaxZ = 7f;

    private List<GameObject> fishPool;
    private float inactiveStartTime = -1f;
    private float inactiveDurationThreshold = 5f;

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
        // Initialize the fish pool
        InitializePool();
        // Activate the initial set of fish
        ActivateInitialFish();
    }

    void Update()
    {
        if (AreAllFishInactive())
        {
            if (inactiveStartTime < 0)
            {
                inactiveStartTime = Time.time;
            }
            else if (Time.time - inactiveStartTime >= inactiveDurationThreshold)
            {
                ActivateInitialFish();
                inactiveStartTime = -1f; // Reset timer after activating fish
            }
        }
        else
        {
            inactiveStartTime = -1f; // Reset timer if any fish is active
        }
    }

    bool AreAllFishInactive()
    {
        return fishPool.All(fish => !fish.activeInHierarchy);
    }

    // creates an object pool of fish ensuring at least 1 of each type of fish
    void InitializePool()
    {
        fishPool = new List<GameObject>();
        int N = fishPrefabs.Length;
        if (N == 0)
        {
            Debug.LogError("fishPrefabs array is empty!");
            return;
        }
        if (poolSize >= N)
        {
            // Add one of each type of fish
            foreach (GameObject prefab in fishPrefabs)
            {
                GameObject fish = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                fish.SetActive(false);
                fishPool.Add(fish);
            }
            // fill with additional random fish
            int remaining = poolSize - N;
            for (int i = 0; i < remaining; i++)
            {
                GameObject selectedPrefab = fishPrefabs[Random.Range(0, N)];
                GameObject fish = Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity);
                fish.SetActive(false);
                fishPool.Add(fish);
            }
        }
        else
        {
            Debug.LogError("poolSize must be >= num of fishPrefabs!");
            return;
        }
    }

    void ActivateInitialFish()
    {
        for (int i = 0; i < initialFishCount && i < poolSize; i++)
        {
            ActivateFishFromPool();
        }
    }

    public void ActivateFishFromPool()
    {
        GameObject fish = GetInactiveFish();
        if (fish != null)
        {
            // Random direction: left or right
            Vector3 randomDir = Random.value < 0.5f ? Vector3.left : Vector3.right;
            // Set x position based on direction
            float x = (randomDir == Vector3.right) ? spawnMinX : spawnMaxX;
            // Random z within bounds
            float z = Random.Range(spawnMinZ, spawnMaxZ);
            // Use prefab's y-position
            float y = fish.transform.position.y;
            Vector3 spawnPos = new Vector3(x, y, z);

            // Position and activate the fish
            fish.transform.position = spawnPos;
            FishMovement fishMovement = fish.GetComponent<FishMovement>();
            if (fishMovement != null)
            {
                fishMovement.InitializeDirection(randomDir);
            }
            fish.SetActive(true);

            Debug.Log($"Fish activated at {spawnPos} swimming {randomDir}");
        }
    }

    GameObject GetInactiveFish()
    {
        foreach (GameObject fish in fishPool)
        {
            if (!fish.activeInHierarchy)
            {
                return fish;
            }
        }
        Debug.LogWarning("No inactive fish available in the pool!");
        return null;
    }
}