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
    public UnityEvent<StateScene> stateScene;   
    [SerializeField] private GameEvent<int> OnEnemyDeath;
    private bool change;
    private StateScene state;
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
        state = StateScene.State1;

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
    public void ChangeState(StateScene state, GameObject player)
    {
        if (!change) return;
        change = false;
        switch (state)
        {
            case StateScene.State1:
                text1.text = "Bien Hecho! Sabes Moverte! Tal vez seas por fin el heroe que busco, el resto o terminaban carbonizados o basura espacial...";
                text2.text = "En fin espero no tener que buscar más. Si pulsas F puedes hacer un dash, lo necesitaras para este tramo.";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(4.14f, 8.26f, 30.98f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                eventEncerrar1?.Invoke(false);
                break;
            case StateScene.State2:
                text1.text = "Bien Ahora que sabes moverte pulsa I para entrar en tu control de arma. Allí podrás configurar tu arma con distintos menus.";
                text3.text = "Ves al campo de pruebas detrás de mi y de momento limitate ha usar los 3 porcentajes para familiarizarte!";
                text2.text = "Hay dos tipos de configuracion. Los porcentajes de: Power, Accuracy y Frequency; y por otra parte el sistema de puntos de PowerBullets.";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(0.39f, 5f, 240f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case StateScene.State3:
                text1.text = "Prueba con estos maniquís las diferentes configuraciones";
                text2.text = "Vamos a ver como te mueves!";
                text3.text = "Cuando creas que ya estás listo pasa a la siguiente plataforma!";
                transform.position = new Vector3(-4.4f, 5f, 270f);
                transform.eulerAngles = new Vector3(0, 0, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                break;
            case StateScene.State4:
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
            case StateScene.State5:
                if (NetworkManager.Singleton.IsServer)
                {
                    if (player.transform.parent.gameObject.GetComponent<NetworkObject>().IsOwner)
                    {
                        player.transform.parent.gameObject.GetComponent<PlayerControlls>().WinPoints(10);
                        text3.text = "";
                        text1.text = "Verás que te acabo de dar 10 puntos de energia.";
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
                        text2.text = "Espabila un poco chaval, que tu compañero lo ha hecho más rápido que tú y el tiene lag.";
                        text3.text = "";
                        text1.color = Color.white;
                        text2.color = Color.white;
                        text3.color = Color.white;
                    }
                }
                else
                {
                    if (player.transform.parent.gameObject.GetComponent<NetworkObject>().IsOwner)
                    {
                        text1.text = "";
                        text2.text = "Te felicito has llegado antes que tu compañero y encima tienes lag! Por lo visto está de moda ser lento";
                        text3.text = "Dile a tu compañero que un cubo rosa kawaii espacial te acaba de dar permiso para insultarle! > : ) ";
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
                        text2.text = "Os he dado a ti y a tu compañero 10 puntos de energía.";
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
            case StateScene.State6:
                text1.text = "Cuidado! Hay un enemigo ahí delante! Ahora con lo que sabes puedes intentar enfrentarte a el.";
                text3.text = "Este alienigena se le llama Thraaxiano, los Thraaxianos son lentos pero suelen haber muchos y hacen mucho daño!";
                text2.text = "Ten cuidado por que suelen intentar rodear a su presa.";
                transform.position = new Vector3(40f, 8.5f, 299f);
                transform.eulerAngles = new Vector3(-10f, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                break;
            case StateScene.State7:
                text1.text = "VAMOS JUGADOR! PEGALE UNA PALIZA! ASI ES, AHORA CON LA DERECHA";
                text3.text = "Y AHORA ESO Y LO OTRO ESO TAMBIEN. MATA DESTROZA";
                text2.text = "MUTILA";
                StartCoroutine(checkIfPlayers(new Vector3(60.7f, 6.35f, 298.19f), StateScene.State7));
                text1.color = Color.red;
                text2.color = Color.red;
                text3.color = Color.red;
                transform.position = new Vector3(80f, 15f, 299f);
                transform.eulerAngles = new Vector3(-20f, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case StateScene.State8:
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
            case StateScene.State9:
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
            case StateScene.State10:
                text1.text = "VAMOS JUGADOR TU PUEDES. NO TE CAGUES ENCIMA";
                text3.text = "SI TE CARGAS A ESTE BICHO TE JURO QUE TE LLAMARE ONICHAN";
                text2.text = "(es broma)";
                text1.color = Color.red;
                text2.color = Color.white;
                text3.color = Color.red;
                StartCoroutine(checkIfPlayers(new Vector3(60.7f, 6.35f, 212.69f), StateScene.State10));
                transform.position = new Vector3(75f, 20f, 210f);
                transform.eulerAngles = new Vector3(-20, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case StateScene.State11:
                text1.text = "No esta nada mal jugador. Parece que vas a sobrevivir y todo.";
                text3.text = "Pero no bajes la guardia, ahora te enfrentaras a un Zorgoniano";
                text2.text = "Venga, avanza y te explicaré su comportamiento!";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(70f, 10f, 210.8f);
                transform.eulerAngles = new Vector3(0, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case StateScene.State12:
                text1.text = "Bien pues solo te queda destruir al Zorgoniano";
                text3.text = "Este a diferencia de los otros ataca cuerpo a cuerpo y hace mucho daño!";
                text2.text = "Ten mucho cuidado jugador! Y mantente a las distancias por que por cada golpe te estunea!";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(65f, 10f, 150f);
                transform.eulerAngles = new Vector3(0, 180, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case StateScene.State13:
                text1.text = "VENGA YA ES TUYO DALE UNA PALIZA, MATALE";
                text3.text = "CARGATE A SU FAMILIA Y CUANDO PIENSE QUE NO PUEDE SUFRIR MAS REVIVELE PARA VOLVER A MATARLE";
                text2.text = "... A lo mejor debería relajarme";
                text1.color = Color.red;
                text2.color = Color.white;
                text3.color = Color.red;
                StartCoroutine(checkIfPlayers(new Vector3(60.7f, 6.35f, 127.3f), StateScene.State13));
                transform.position = new Vector3(75f, 20f, 135f);
                transform.eulerAngles = new Vector3(-20, 90, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            case StateScene.State14:
                text1.text = "Bien hecho, creo que ya estás mas que listo para jugar.";
                text3.text = "Ahora parte en tu viaje heroe, y hazte leyenda.";
                text2.text = "El cosmos entero esta en peligro y tu eres su unica esperanza. ";
                text1.color = Color.white;
                text2.color = Color.white;
                text3.color = Color.white;
                transform.position = new Vector3(65f, 10f, 100f);
                transform.eulerAngles = new Vector3(0, 180, 0);
                currentY = transform.position.y;
                goToY = currentY + 1;
                break;
            default:
                break;
        }
        change = true;
    }
    private IEnumerator checkIfPlayers(Vector3 v, StateScene state)
    {
        yield return new WaitForSeconds(1);
        int players = 0;
        Collider[] colliders = Physics.OverlapSphere(v, 18f);

        foreach (Collider collider in colliders)
        {
            if (collider.transform.tag == "Player")
            {
                players++;
            }
            if (players == info.NumberOfPlayers)
            {
                eventEncerrar1?.Invoke(true);
                stateScene?.Invoke(state);
                break;
            }
        }
    }
}
public enum StateScene
{
    State1, State2, State3, State4, State5,State6,State7,State8,State9, State10,State11,State12,State13,State14
}