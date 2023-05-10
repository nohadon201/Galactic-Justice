using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Tutorialer : MonoBehaviour, IEventListener
{
    private float currentY;
    private float goToY;
    private bool up;
    [SerializeField] private Vector3 NexPosition;
    TextMeshProUGUI text1;
    TextMeshProUGUI text2;
    TextMeshProUGUI text3;
    public UnityEvent<bool> eventEncerrar1;
    [SerializeField] private GameEvent<int> OnEnemyDeath;
    private bool change;
    private TutorialerState state;
    private MultiplayerInfo info;

    void Awake()
    {
        OnEnemyDeath.RegisterListener(this);
        info = Resources.Load<MultiplayerInfo>("Multiplayer/MultiplayerInfo");
        change = true;
        currentY = transform.position.y;
        goToY = currentY + 1;
        up = true;
        text1 = transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        text2 = transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        text3 = transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        state = TutorialerState.State1;

    }

    // Update is called once per frame
    void Update()
    {
        if(((goToY - transform.position.y <= 0.5f) && (goToY - transform.position.y >= -0.5f)) && up)
        {
            goToY = currentY - 1;
            up = false;
        }else if ((goToY - transform.position.y <= 0.5f) && (goToY - transform.position.y >= -0.5f))
        {
            goToY = currentY + 1;
            up = true;
        }
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, goToY, transform.position.z),0.004f);
    }
    public void OnDisparar()
    {
        text1.text = "Jajajajaj eres muy gracioso jugador kun! Pero eso no es un juguete! :D Ah por cierto...";
        text3.text = "";
        text2.text = "no te pases por que tengo a tu familia amordazada en mi sotano... Jjajaja es bromi (o no).";
        text2.color = Color.red;
    }
    public void ChangeState(TutorialerState state, GameObject player)
    {
        if (!change) return;
        change = false;
        switch (state)
        {
            case TutorialerState.State1:
                text1.text = "Bien Hecho! Sabes Moverte! Tal vez seas por fin el heroe que busco, el resto o terminaban carbonizados o basura espacial...";
                text2.text = "En fin espero no tener que buscar m�s. Si pulsas F puedes hacer un dash, lo necesitaras para este tramo.";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(4.14f, 8.26f, 30.98f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                eventEncerrar1?.Invoke(false);
                break;
            case TutorialerState.State2:
                text1.text = "Bien Ahora que sabes moverte pulsa I para entrar en tu control de arma. All� podr�s configurar tu arma con distintos menus.";
                text3.text = "Ves al campo de pruebas detr�s de mi y de momento limitate ha usar los 3 porcentajes para familiarizarte!";
                text2.text = "Hay dos tipos de configuracion. Los porcentajes de: Power, Accuracy y Frequency; y por otra parte el sistema de puntos de PowerBullets.";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(0.39f, 5f, 240f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case TutorialerState.State3:
                text1.text = "Prueba con estos maniqu�s las diferentes configuraciones";
                text2.text = "Vamos a ver como te mueves!";
                text3.text = "Cuando creas que ya est�s listo pasa a la siguiente plataforma!";
                transform.position = new Vector3(-4.4f, 5f, 270f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                break;
            case TutorialerState.State4:
                text1.text = "";
                text2.text = "Bien! Avancemos!";
                text3.text = "";
                transform.position = new Vector3(-4.4f, 1.32f, 290f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                break;
            case TutorialerState.State5:
                if (NetworkManager.Singleton.IsServer)
                {
                    if (player.transform.parent.gameObject.GetComponent<NetworkObject>().IsOwner)
                    {
                        player.transform.parent.gameObject.GetComponent<PlayerControlls>().WinPoints(10);
                        text3.text = "";
                        text1.text = "Ver�s que te acabo de dar 10 puntos de energia.";
                        text2.text = "Prueba a configurar tu arma con los PowerBullets entrando en el menu con I.";
                        transform.position = new Vector3(12.41126f, 1.32f, 295f);
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        
                        currentY = transform.position.y;
                        goToY = currentY + 1;
                        text1.color = Color.white;
                        text2.color = Color.white;
                        text3.color = Color.white;
                    }
                    else
                    {
                        text1.text = "";
                        text2.text = "Espabila un poco chaval, que tu compa�ero lo ha hecho m�s r�pido que t� y el tiene lag.";
                        text3.text = "";
                        text1.color = Color.white;
                        text2.color = Color.white;
                        text3.color = Color.white;
                    }
                }
                else
                {
                    if (player.GetComponent<NetworkObject>().IsOwner)
                    {
                        text1.text = "";
                        text2.text = "Te felicito has llegado antes que tu compa�ero y encima tienes lag! Por lo visto est� de moda ser lento";
                        text3.text = "Dile a tu compa�ero que un cubo rosa kawaii espacial te acaba de dar permiso para insultarle! > : ) ";
                        transform.position = new Vector3(-4.4f, 1.32f, 307f);
                        currentY = transform.position.y;
                        goToY = currentY + 1;
                        text1.color = Color.white;
                        text2.color = Color.white;
                        text3.color = Color.white;
                    }
                    else
                    {
                        text1.text = "";
                        text2.text = "Os he dado a ti y a tu compa�ero 10 puntos de energ�a.";
                        text3.text = "Probad algunos powerBullets a vuestro gusto!";
                        transform.position = new Vector3(12.41126f, 1.32f, 295f);
                        transform.eulerAngles = new Vector3(0, 90, 0);
                        currentY = transform.position.y;
                        goToY = currentY + 1;
                        text1.color = Color.white;
                        text2.color = Color.white;
                        text3.color = Color.white;
                    }
                }
                break;
            case TutorialerState.State6:
                text1.text = "Cuidado! Hay un enemigo ah� delante! Ahora con lo que sabes puedes intentar enfrentarte a el.";
                text3.text = "Este alienigena se le llama Thraaxiano, los Thraaxianos son lentos pero suelen haber muchos y hacen mucho da�o!";
                text2.text = "Ten cuidado por que suelen intentar rodear a su presa.";
                transform.position = new Vector3(40f, 8.5f, 299f);
                transform.eulerAngles = new Vector3(-10f, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                break;
            case TutorialerState.State7:
                text1.text = "VAMOS JUGADOR! PEGALE UNA PALIZA! ASI ES, AHORA CON LA DERECHA";
                text3.text = "Y AHORA ESO Y LO OTRO ESO TAMBIEN. MATA DESTROZA";
                text2.text = "MUTILA";
                StartCoroutine(checkIfPlayers());
                text1.color = Color.red;
                text2.color = Color.red;
                text3.color = Color.red;
                transform.position = new Vector3(80f, 15f, 299f);
                transform.eulerAngles = new Vector3(-20f, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case TutorialerState.State8:
                text1.text = "Muy bien hecho la verdad, no me esperaba que fueras tan capaz...";
                text3.text = "Vale me has pillado puede que te subestimase";
                text2.text = "Pasemos a la siguiente plataforma!";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(75f, 10f, 299f);
                transform.eulerAngles = new Vector3(0, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case TutorialerState.State9:
                text1.text = "Bien! Este enemigo que vamos a encontrar se le llama Quiraxiano, ataca a distancia al igual que el Thraaxiano";
                text3.text = "Pero sus ataques son mas rapidos y tiene mas resistencia";
                text2.text = "A por el!";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(65f, 10f, 245.8f);
                transform.eulerAngles = new Vector3(0, 180, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            default:
                break;
        }
        change = true;
    }
    private IEnumerator checkIfPlayers()
    {
        yield return new WaitForSeconds(1);
        int players = 0;
        Collider[] colliders = Physics.OverlapSphere(new Vector3(60.7f, 6.35f, 298.19f), 18f);

        foreach (Collider collider in colliders)
        {
            if (collider.transform.tag == "Player")
            {
                players++;
            }
            if (players == info.NumberOfPlayers)
            {
                eventEncerrar1?.Invoke(true);
                break;
            }
        }
    }
}
public enum TutorialerState
{
    State1, State2, State3, State4, State5,State6,State7,State8,State9
}