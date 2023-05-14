using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserSession : NetworkBehaviour
{
    private GameObject Player;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public override void OnNetworkSpawn()
    {
        MultiplayerInfo info = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");
        info.NumberOfPlayers++;
        info.connected= true;   
    }
    public override void OnNetworkDespawn()
    {
        MultiplayerInfo info = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");
        info.NumberOfPlayers--;
        info.connected = false;
    }
    private void OnLevelWasLoaded(int level)
    {
        if (!IsOwner) return;
        if (SceneManager.GetSceneByBuildIndex(level).name != "Menu" && SceneManager.GetSceneByBuildIndex(level).name != "LevelMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            SpawnPlayerServerRpc(this.GetComponent<NetworkBehaviour>().OwnerClientId);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    [ServerRpc]
    public void SpawnPlayerServerRpc(ulong OwnerClientId)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Player/Player"));
        //go.transform.position = new Vector3(61.3f, 8.5f, 165.9f);
        //go.transform.position = new Vector3(0.08f, 2f, 298.19f);
        go.transform.position = new Vector3(0f, 0f, 0f);
        go.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true) ;
    } 
    void Update()
    {
        
    }
}
