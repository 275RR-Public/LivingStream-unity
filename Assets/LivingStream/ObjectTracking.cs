using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine.Rendering.HighDefinition;

// Define a serializable class for individual tracking data
[System.Serializable]
public class TrackData
{
    public int id;
    public float[] position; // Array for [x, y, z] or null
}

// Define a serializable wrapper for the list of tracking data
[System.Serializable]
public class TrackingDataList
{
    public List<TrackData> tracks;
}

public class ObjectTracking : MonoBehaviour
{
    public GameObject trackedPersonPrefab; // Assign in Inspector with an empty game object, "TrackingManager"
    public WaterSurface river;          // Assign the river GameObject to TrackingManager
    private UdpClient udpClient;
    private Dictionary<int, GameObject> trackedObjects = new Dictionary<int, GameObject>();
    private ConcurrentQueue<List<TrackData>> trackingDataQueue = new ConcurrentQueue<List<TrackData>>();
    private Dictionary<int, float> lastSeenTimes = new Dictionary<int, float>();
    private float timeoutDuration = 2f;
    private Dictionary<int, Vector3> targetPositions = new Dictionary<int, Vector3>();
    private int frameCounter = 0;
    private const int checkInterval = 10;
    private bool isClosing = false;

    void Start()
    {
        udpClient = new UdpClient(5005);
        udpClient.BeginReceive(ReceiveCallback, null);

        if (trackedPersonPrefab == null)
        {
            Debug.LogWarning("trackedPersonPrefab is not assigned! Creating a default cube.");
            trackedPersonPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackedPersonPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (river == null)
        {
            Debug.LogWarning("river WaterSurface not assigned in ObjectTracking!");
        }
    }

    void ReceiveCallback(System.IAsyncResult ar)
    {
        if (isClosing)
            return;

        try
        {
            var endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 5005);
            byte[] data = udpClient.EndReceive(ar, ref endpoint);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log("Received UDP message: " + message);

            string wrappedMessage = "{\"tracks\":" + message + "}";
            Debug.Log("Wrapped message: " + wrappedMessage);

            TrackingDataList trackingDataList = JsonUtility.FromJson<TrackingDataList>(wrappedMessage);
            List<TrackData> trackingData = trackingDataList.tracks;
            Debug.Log("Deserialized tracking data count: " + trackingData.Count);

            trackingDataQueue.Enqueue(trackingData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in ReceiveCallback: " + e.Message);
        }

        if (!isClosing)
            udpClient.BeginReceive(ReceiveCallback, null);
    }

    void Update()
    {
        HashSet<int> activeIds = new HashSet<int>();
        List<TrackData> latestTrackingData = null;
        while (trackingDataQueue.TryDequeue(out List<TrackData> trackingData))
        {
            latestTrackingData = trackingData;
        }

        if (latestTrackingData != null)
        {
            foreach (var track in latestTrackingData)
            {
                int id = track.id;
                activeIds.Add(id);
                lastSeenTimes[id] = Time.time;
                Debug.Log($"Processing track ID: {id}, Position: {(track.position != null ? string.Join(", ", track.position) : "null")}");

                if (track.position != null && track.position.Length == 3)
                {
                    Vector3 realSensePos = new Vector3(track.position[0], track.position[1], track.position[2]);
                    Debug.Log($"RealSense position: {realSensePos}");

                    Vector3 unityPos = new Vector3(realSensePos.x, 0f, realSensePos.z);
                    Debug.Log($"Unity position: {unityPos}");

                    targetPositions[id] = unityPos;

                    if (!trackedObjects.ContainsKey(id))
                    {
                        GameObject newBox = Instantiate(trackedPersonPrefab);
                        newBox.name = "TrackedPerson_" + id;
                        trackedObjects[id] = newBox;

                        // Assign the river to the FloatingIceberg script
                        FloatingIceberg floatingIceberg = newBox.GetComponent<FloatingIceberg>();
                        if (floatingIceberg != null && river != null)
                        {
                            floatingIceberg.targetSurface = river;
                        }
                        else
                        {
                            Debug.LogWarning($"Could not assign WaterSurface to TrackedBox_{id}. FloatingIceberg or river is missing.");
                        }

                        Material newMaterial = new Material(Shader.Find("HDRP/Lit"));
                        float hue = (id % 10) / 10f;
                        newMaterial.color = Color.HSVToRGB(hue, 1f, 1f);
                        newBox.GetComponent<MeshRenderer>().material = newMaterial;
                        Debug.Log($"Instantiated new box for ID {id} at position {unityPos}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Track ID {id} has invalid position data.");
                }
            }
        }

        frameCounter++;
        if (frameCounter >= checkInterval)
        {
            frameCounter = 0;
            RemoveTracks(activeIds);
        }
    }

    private void RemoveTracks(HashSet<int> activeIds)
    {
        List<int> idsToRemove = new List<int>();
        foreach (var id in trackedObjects.Keys)
        {
            if (!activeIds.Contains(id) && (Time.time - lastSeenTimes.GetValueOrDefault(id, 0f) > timeoutDuration))
            {
                idsToRemove.Add(id);
            }
        }

        foreach (var id in idsToRemove)
        {
            Destroy(trackedObjects[id]);
            trackedObjects.Remove(id);
            lastSeenTimes.Remove(id);
            targetPositions.Remove(id);
            Debug.Log($"Removed tracked object for ID {id}");
        }
    }

    void LateUpdate()
    {
        foreach (var kvp in trackedObjects)
        {
            int id = kvp.Key;
            GameObject obj = kvp.Value;
            if (targetPositions.ContainsKey(id))
            {
                Vector3 target = targetPositions[id];
                Vector3 currentPos = obj.transform.position;
                Vector3 targetXZ = new Vector3(target.x, currentPos.y, target.z);
                obj.transform.position = Vector3.Lerp(currentPos, targetXZ, Time.deltaTime * 5f);
            }
        }
    }

    void OnDestroy()
    {
        isClosing = true;
        if (udpClient != null)
            udpClient.Close();
    }
}