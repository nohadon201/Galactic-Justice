using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SaveGameManager : NetworkBehaviour
{
    private static SaveGameManager instance;
    public static SaveGameManager Singleton { get { return instance; } }
    private SaveGame saveGame;
    private void Awake()
    {
        if(instance == null)
        {
            Destroy(gameObject);
            return;
        }   
        instance = this; 
        saveGame = Resources.Load<SaveGame>("System/SaveGameSO");
        saveGame.playerHost.DefaultValues(true);
        saveGame.playerClient.DefaultValues(false);
        DontDestroyOnLoad(gameObject);  
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    [ClientRpc]
    public void SaveClientRpc()
    {

    }
}
