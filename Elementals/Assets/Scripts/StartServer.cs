using Unity.Netcode;
using UnityEngine;

public class StartServer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.StartServer();
    }
}
