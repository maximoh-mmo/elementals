using Unity.Netcode;
using UnityEngine;

public class StartServer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            NetworkManager.Singleton.StartServer();
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req,
        NetworkManager.ConnectionApprovalResponse resp)
    {
        var clientId = req.ClientNetworkId;
        var connectionData = req.Payload;
        resp.Approved = true;
        for (int i = 0 ;  i < connectionData.Length ; i++)
        {
            if (connectionData[i] != System.Text.Encoding.ASCII.GetBytes("secretpassword")[i])
            {
                resp.Approved = false;
            }
        }
        resp.Position = InstantiateRandomPosition(0);
        resp.CreatePlayerObject = true;
    }

    Vector3 GetPlayerSpawnPosition(ulong ClientNetworkId)
    {
        /**
         * Here we look for details of the player based on client id
         * Establish db connection and query for clientId, if found return saved
         * client position from last session. if not found goto default spawn position
        **/
        return InstantiateRandomPosition(0);
    }

    Vector3 InstantiateRandomPosition(float offset)
    {
        Terrain terrain = Terrain.activeTerrain;
        LayerMask mask = LayerMask.GetMask("Terrain", "Water");
        Vector3 randomPosition = new Vector3();
        RaycastHit hit;
        do
        {
            randomPosition.x = Random.Range(terrain.transform.position.x, terrain.transform.position.x + terrain.terrainData.size.x);
            randomPosition.z = Random.Range(terrain.transform.position.y, terrain.transform.position.y + terrain.terrainData.size.y);
            Debug.Log("RandomPosition= " +randomPosition.x +", " + randomPosition.y+", " + randomPosition.z);
            if (Physics.Raycast(new Vector3(randomPosition.x, 9999f, randomPosition.z), Vector3.down, out hit, Mathf.Infinity,
                    mask))
            {
                randomPosition.y = hit.point.y;
            }
        } while (randomPosition.y == 0);
        return randomPosition;
    }
}
