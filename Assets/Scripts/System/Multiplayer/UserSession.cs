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
        Debug.Log("aa");
        MultiplayerInfo info = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");
        info.NumberOfPlayers++;
        info.connected= true;   
    }
    private void OnLevelWasLoaded(int level)
    {
        
        if (SceneManager.GetSceneByBuildIndex(level).name != "Menu" && SceneManager.GetSceneByBuildIndex(level).name != "LevelMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            SpawnPlayerServerRpc(this.GetComponent<NetworkBehaviour>().OwnerClientId);    
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAA");
        }
    }
    [ServerRpc]
    public void SpawnPlayerServerRpc(ulong OwnerClientId)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        go.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
    } 
    void Update()
    {
        
    }
}
