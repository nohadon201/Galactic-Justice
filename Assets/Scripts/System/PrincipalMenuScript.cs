
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrincipalMenuScript : MonoBehaviour
{
    public MultiplayerInfo MultiplayerInfo;
    private void Awake()
    {
        MultiplayerInfo = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");
    }
    public void StartGame()
    {
        MultiplayerInfo.Host = true;
        SceneManager.LoadScene("LevelMenu");
    }
    public void JoinGame()
    {
        MultiplayerInfo.Host = false;
        SceneManager.LoadScene("LevelMenu");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}