using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;

[System.Serializable]
public class TrackData
{
    public int id;
    public float[] position;
}

[System.Serializable]
public class TrackingDataList
{
    public List<TrackData> tracks;
}

public class UdpListener : MonoBehaviour
{
    private static UdpListener instance;
    public static UdpListener Instance => instance;

    private UdpClient udpClient;
    private ConcurrentQueue<List<TrackData>> trackingDataQueue = new ConcurrentQueue<List<TrackData>>();
    private bool isClosing = false;

    void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        try
        {
            udpClient = new UdpClient(5005);
            udpClient.BeginReceive(ReceiveCallback, null);
            Debug.Log("UdpListener started listening on port 5005");
        }
        catch (SocketException e)
        {
            Debug.LogError("Failed to start UdpListener: " + e.Message);
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
            TrackingDataList trackingDataList = JsonUtility.FromJson<TrackingDataList>(wrappedMessage);
            List<TrackData> trackingData = trackingDataList.tracks;
            trackingDataQueue.Enqueue(trackingData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in ReceiveCallback: " + e.Message);
        }

        if (!isClosing)
            udpClient.BeginReceive(ReceiveCallback, null);
    }

    public bool TryDequeueTrackingData(out List<TrackData> trackingData)
    {
        return trackingDataQueue.TryDequeue(out trackingData);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            isClosing = true;
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
            }
            instance = null;
        }
    }

    void OnApplicationQuit()
    {
        isClosing = true;
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
