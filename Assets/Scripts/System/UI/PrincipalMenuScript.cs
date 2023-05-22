
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrincipalMenuScript : MonoBehaviour
{

    public MultiplayerInfo MultiplayerInfo;
    private void Awake()
    {
        MultiplayerInfo = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");
        MultiplayerInfo.NumberOfPlayers = 0;
        if (SaveGameManager.Singleton.ExistOneSavedGame())
        {
            for (int a = 1; a < 6; a++)
            {
                ShowSavedGameInfo show = SaveGameManager.Singleton.GetSavedGameInfo(a);
                if (show.LevelsCompleted != -1)
                {
                    GameObject go = transform.GetChild(1).GetChild(a - 1).gameObject;
                    Button button = go.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    switch (a)
                    {
                        case 1:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadConfiguration(1, MultiplayerInfo));
                            break;

                        case 2:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadConfiguration(2, MultiplayerInfo));
                            break;

                        case 3:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadConfiguration(3, MultiplayerInfo));
                            break;

                        case 4:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadConfiguration(4, MultiplayerInfo));
                            break;

                        case 5:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadConfiguration(5, MultiplayerInfo));
                            break;
                        default: break;
                    }
                    button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                    button.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Levels: " + show.LevelsCompleted;
                    button.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "Points: " + show.PointsOfPlayer;
                    button.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "Date: " + show.DataCreation;
                }
                else
                {
                    GameObject go = transform.GetChild(1).GetChild(a - 1).gameObject;
                    Button button = go.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Slot " + a + " Empty";
                    button.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                    button.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                    button.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                }
            }
        }
        else
        {
            transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        }
        if (SaveGameManager.Singleton.ExistOneSavedCharacter())
        {
            for (int a = 1; a < 6; a++)
            {
                ShowSavedCharacter show = SaveGameManager.Singleton.GetSavedCharacter(a);
                if (show.PointsOfPlayer != -1)
                {
                    GameObject go = transform.GetChild(2).GetChild(a - 1).gameObject;
                    Button button = go.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    switch (a)
                    {
                        case 1:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadCharacter(1, MultiplayerInfo));
                            break;

                        case 2:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadCharacter(2, MultiplayerInfo));
                            break;

                        case 3:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadCharacter(3, MultiplayerInfo));
                            break;

                        case 4:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadCharacter(4, MultiplayerInfo));
                            break;

                        case 5:
                            button.onClick.AddListener(() => SaveGameManager.Singleton.LoadCharacter(5, MultiplayerInfo));
                            break;
                        default: break;
                    }
                    button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Points: " + show.PointsOfPlayer;
                    button.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Last Play: " + show.lastDayConnected;
                }
                else
                {
                    GameObject go = transform.GetChild(2).GetChild(a - 1).gameObject;
                    Button button = go.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    switch (a)
                    {
                        case 1:
                            button.onClick.AddListener(()=>LoadCharacter(1));
                            break;

                        case 2:
                            button.onClick.AddListener(() => LoadCharacter(2));
                            break;

                        case 3:
                            button.onClick.AddListener(() => LoadCharacter(3));
                            break;

                        case 4:
                            button.onClick.AddListener(() => LoadCharacter(4));
                            break;

                        case 5:
                            button.onClick.AddListener(() => LoadCharacter(5));
                            break;
                        default: break;
                    }
                    button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Slot Empty";
                    button.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                }
            }
        }

    }
    public void LoadCharacter(int a)
    {
        SaveGameManager.Singleton.SlotOFSaveGame = a;
        MultiplayerInfo.Host = false;
        SceneManager.LoadScene("LevelMenu");

    }
    public void StartGame()
    {
        if (!SaveGameManager.Singleton.CheckIfLimitMemory())
        {
            MultiplayerInfo.Host = true;
            SceneManager.LoadScene("LevelMenu");
        }

    }
    public void JoinGame()
    {
        if (SaveGameManager.Singleton.ExistOneSavedCharacter())
        {
            DisplayMenu();
        }
        else
        {
            SaveGameManager.Singleton.SlotOFSaveGame = 1;
            MultiplayerInfo.Host = false;
            SceneManager.LoadScene("LevelMenu");
        }

    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void DisplayMenuSavedGames()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
    }
    public void DisplayMenu()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
    }
    public void GoBack()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}