using System;
using Unity.Multiplayer.PlayMode;
using Unity.Netcode;
using UnityEngine;

public class TestServer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 메인 에디터인 경우 서버로 시작
        if (CurrentPlayer.IsMainEditor)
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log($"[Editor Server] Main Editor started as Server at {DateTime.UtcNow}");
        }
        // 그 외 가상 플레이어들은 클라이언트로 시작
        else
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log($"[Editor Client] Virtual Player started as Client at {DateTime.UtcNow}");
        }
    }
}
