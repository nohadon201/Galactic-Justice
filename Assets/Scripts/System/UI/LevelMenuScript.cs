using System;
using System.Net.Sockets;
using System.Net;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelMenuScript : MonoBehaviour
{
    private MultiplayerInfo multiplayerInfo;
    [SerializeField] private Button btnStartConnection;
    [SerializeField] private GameObject IpField;
    private TMP_InputField inputIp;
    [SerializeField] private TextMeshProUGUI StatusText;
    private TextMeshProUGUI NumbOfPlayersCount;
    private String ipString;
    public void Awake()
    {
        multiplayerInfo = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");

        if (!multiplayerInfo.Host)
        {
            StatusText.text = "";
        }
        else
        {
            setIp();
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipString;
        }
        
        inputIp = IpField.GetComponent<TMP_InputField>();
        
        btnStartConnection.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = inputIp.text;
            NetworkManager.Singleton.StartClient();
            btnStartConnection.gameObject.SetActive(false);
            IpField.SetActive(false);
            StatusText.text = "Wait while connecting...";
        });
        
        NumbOfPlayersCount = transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>();

        StartCoroutine(CheckPlayerNumb());
    }
    void Start()
    {
        if (!multiplayerInfo.Host)
        {
            btnStartConnection.gameObject.SetActive(true);
            IpField.SetActive(true);
            transform.GetChild(4).gameObject.SetActive(false);
            StatusText.text = "Please put the Ip provided in the host Menu wich you want to connect";
        }
        else
        {
            btnStartConnection.gameObject.SetActive(false);
            IpField.SetActive(false);
            NetworkManager.Singleton.StartHost();
        }
    }
    public void BackToMenu(InputAction.CallbackContext callbackContext)
    {
        
    }
    public void setIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                StatusText.text = "Your current Ip is: " + ip.ToString();
                this.ipString = ip.ToString();
            }
        }

    }
    public IEnumerator CheckPlayerNumb()
    {
        while(true) {
            if(multiplayerInfo.Host)
            {
                NumbOfPlayersCount.text = "Current number of Players: " + multiplayerInfo.NumberOfPlayers;
            }
            else if(multiplayerInfo.connected && StatusText.text == "Wait while connecting...")
            {
                StatusText.text = "Connected to " + inputIp.text;
                NumbOfPlayersCount.text = "The host is choosing wich level play";
            }
            
            yield return new WaitForSeconds(1f);  
        }
    }

    public void LoadLevel1()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Test2", LoadSceneMode.Single);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Menu");
        }
    }
}
