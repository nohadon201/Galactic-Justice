using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGameManager : NetworkBehaviour
{
    private string PathProgramSave;
    private string PathHostData;
    private string PathClientData;
    public int SlotOFSaveGame;
    private static SaveGameManager instance;
    public static SaveGameManager Singleton { get { return instance; } }
    public SaveGame saveGame;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }   
        instance = this; 
        saveGame = Resources.Load<SaveGame>("System/SaveGameSO");
        saveGame.playerHost.DefaultValues(true);
        saveGame.playerClient.DefaultValues(false);
        DontDestroyOnLoad(gameObject);
        PathProgramSave = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/NohadonIndustries/GalacticJustice";
        PathHostData = PathProgramSave + "/HostSavedGames";
        PathClientData = PathProgramSave + "/ClientSavedCharacters";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            SaveClientRpc();
        }
    }
    public bool CheckIfLimitMemory()
    {
        for (int i = 1; i < 6; i++)
        {
            if (!File.Exists(PathHostData+"/SavedGame" + i + ".json"))
            {
                SlotOFSaveGame = i;
                return false;
            }
        }
        return true;
    }

    public bool ExistOneSavedGame()
    {
        for(int a = 1; a < 6; a++)
        {
            if(File.Exists(PathHostData + "/SavedGame" + a + ".json"))
            {
                return true;
            }
        }
        return false;
    }
    public bool ExistOneSavedCharacter()
    {
        for (int a = 1; a < 6; a++)
        {
            if (File.Exists(PathClientData+"/Character" + a + ".json"))
            {
                return true;
            }
        }
        return false;
    }
    public ShowSavedGameInfo GetSavedGameInfo(int index)
    {
        if (!File.Exists(PathHostData+"/SavedGame" + index + ".json")) return new ShowSavedGameInfo(-1,-1, "");
        FileInfo info = new FileInfo(PathHostData + "/SavedGame" + index + ".json");
        string base64 = File.ReadAllText(PathHostData + "/SavedGame" + index + ".json");
        byte[] data = System.Convert.FromBase64String(base64);
        SaveGame saveGame = SerializationUtility.DeserializeValue<SaveGame>(data, DataFormat.JSON);
        ShowSavedGameInfo show = new ShowSavedGameInfo(saveGame.LevelsCompleted, saveGame.playerHost.TotalPoints, info.CreationTime.ToString());
        return show;
    }
    public ShowSavedCharacter GetSavedCharacter(int index)
    {
        if (!File.Exists(PathClientData + "/Character" + index + ".json")) return new ShowSavedCharacter(-1, "");
        FileInfo info = new FileInfo(PathClientData + "/Character" + index + ".json");
        string base64 = File.ReadAllText(PathClientData+"/Character" + index + ".json");
        byte[] data = System.Convert.FromBase64String(base64);
        SaveGame saveGame = SerializationUtility.DeserializeValue<SaveGame>(data, DataFormat.JSON);
        ShowSavedCharacter show = new ShowSavedCharacter(saveGame.playerClient.TotalPoints, info.LastWriteTime.ToString());
        return show;
    }
    public void LoadConfiguration(int index, MultiplayerInfo MultiplayerInfo)
    {
        string base64 = File.ReadAllText(PathHostData+"/SavedGame" + index + ".json");
        byte[] data = System.Convert.FromBase64String(base64);
        SaveGame saveGame = SerializationUtility.DeserializeValue<SaveGame>(data, DataFormat.JSON);
        this.saveGame.playerHost.TotalPoints = saveGame.playerHost.TotalPoints;
        this.saveGame.playerHost.Points = saveGame.playerHost.Points;
        
        int e = 0;
        foreach (SlotOfMemory slot in saveGame.playerHost.MemorySlots)
        {
            this.saveGame.playerHost.MemorySlots[e].Power = slot.Power;
            this.saveGame.playerHost.MemorySlots[e].Accuracy = slot.Accuracy;
            this.saveGame.playerHost.MemorySlots[e].Frequency = slot.Frequency;
            e++;
        }
        
        int a = 0;
        foreach(PowerBulletSO powerBullet in saveGame.powerBulletHost)
        {
            this.saveGame.powerBulletHost[a].currentInvestmentValue = powerBullet.currentInvestmentValue;
            this.saveGame.powerBulletHost[a].Points = powerBullet.Points;
            a++;
        }
        this.saveGame.LevelsCompleted = saveGame.LevelsCompleted;
        MultiplayerInfo.Host = true;
        SceneManager.LoadScene("LevelMenu");
    }
    public void LoadCharacter(int index, MultiplayerInfo MultiplayerInfo)
    {
        string base64 = File.ReadAllText(PathClientData + "/Character" + index + ".json");
        byte[] data = System.Convert.FromBase64String(base64);
        SaveGame saveGame = SerializationUtility.DeserializeValue<SaveGame>(data, DataFormat.JSON);
        this.saveGame.playerClient.TotalPoints = saveGame.playerClient.TotalPoints;
        this.saveGame.playerClient.Points = saveGame.playerClient.Points;
        
        int e = 0;
        foreach (SlotOfMemory slot in saveGame.playerClient.MemorySlots)
        {
            this.saveGame.playerClient.MemorySlots[e].Power = slot.Power;
            this.saveGame.playerClient.MemorySlots[e].Accuracy = slot.Accuracy;
            this.saveGame.playerClient.MemorySlots[e].Frequency = slot.Frequency;
            e++;
        }
        
        int a = 0;
        foreach (PowerBulletSO powerBullet in saveGame.powerBulletClient)
        {
            this.saveGame.powerBulletClient[a].currentInvestmentValue = powerBullet.currentInvestmentValue;
            this.saveGame.powerBulletClient[a].Points = powerBullet.Points;
            a++;
        }
        MultiplayerInfo.Host = false;
        SceneManager.LoadScene("LevelMenu");
    }
    [ClientRpc]
    public void SaveClientRpc()
    {
        byte[] listByte = SerializationUtility.SerializeValue<SaveGame>(saveGame, DataFormat.JSON);
        string base64 = System.Convert.ToBase64String(listByte);
        if (!Directory.Exists(PathProgramSave))
        {
            Directory.CreateDirectory(PathProgramSave);
            Directory.CreateDirectory(PathClientData);
            Directory.CreateDirectory(PathHostData);
        }
        
        if (IsServer)
            File.WriteAllText(PathHostData + "/SavedGame" + SlotOFSaveGame + ".json", base64);
        else
            File.WriteAllText(PathHostData + "/Character" + SlotOFSaveGame + ".json", base64);
    }
}
public struct ShowSavedGameInfo
{
    public int LevelsCompleted;
    public int PointsOfPlayer;
    public string DataCreation;
    public ShowSavedGameInfo(int level, int points, string date)
    {
        LevelsCompleted = level;
        PointsOfPlayer = points;
        DataCreation = date;
    }
}
public struct ShowSavedCharacter
{
    public int PointsOfPlayer;
    public string lastDayConnected;
    public ShowSavedCharacter(int Points, string date)
    {
        PointsOfPlayer = Points;
        lastDayConnected= date; 
    }
}