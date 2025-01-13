using Unity.Netcode;
using UnityEngine;

public class MovementNetworkController : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new();
    [SerializeField]
    private float MovementSpeed = 5.0f;
    [Rpc(SendTo.Server)]
    void PositionUpdateServerRpc(Vector3 position, RpcParams rpcParams = default) => Position.Value = position;
    
    void Update(){
        if (IsOwner && !IsServer)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveX, 0, moveZ) * (MovementSpeed * Time.deltaTime);
            transform.Translate(movement, Space.World);
            PositionUpdateServerRpc(transform.position);
        }

        if (IsServer)
        {
            transform.position = Position.Value;
        }
    } 
}