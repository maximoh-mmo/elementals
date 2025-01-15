using System;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkUptime : NetworkBehaviour
{
    private NetworkVariable<float> _networkUptime = new();
    private float _lastT;
    private NetworkVariable<int> _numClients = new();
    [SerializeField]
    private TextMeshProUGUI upTimeValue;
    [SerializeField]
    private TextMeshProUGUI numClientsValue
        ;
    private void Start()
    {
        Assert.IsNotNull(upTimeValue);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _networkUptime.Value = 0f;
            _numClients = new NetworkVariable<int>(NetworkManager.Singleton.ConnectedClients.Count);
            //Debug.Log("Server Uptime variable initialized to " + _networkUptime.Value);
            //Debug.Log("Server Uptime variable initialized to " + _numClients.Value);

        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            _numClients = new NetworkVariable<int>(NetworkManager.Singleton.ConnectedClients.Count);
        }
    }

    private void Update()
    {
        var t_now = Time.time;
        if (IsServer)
        {
            var clients = NetworkManager.Singleton.ConnectedClients.Count;
            _numClients = new NetworkVariable<int>(clients);
            _networkUptime.Value += 0.1f;
            if (t_now - _lastT > 0.5f)
            {
                _lastT = t_now;
                //Debug.Log("Server Uptime variable updated to: " + _networkUptime.Value);
            }
        }
        if (!IsServer)
        {
            upTimeValue.text = _networkUptime.Value.ToString("0.00");
            numClientsValue.text = _numClients.Value.ToString();
        }
    }
}