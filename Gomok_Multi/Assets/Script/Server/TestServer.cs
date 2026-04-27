using System;
using Unity.Netcode;
using UnityEngine;

public class TestServer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_SERVER && !UNITY_EDITOR
        NetworkManager.Singleton.StartServer();
        Debug.Log($"Server Started {DateTime.UtcNow}");
#endif
#if UNITY_EDITOR
        NetworkManager.Singleton.StartClient();
        Debug.Log($"Client Started {DateTime.UtcNow}");
#endif
    }
}
