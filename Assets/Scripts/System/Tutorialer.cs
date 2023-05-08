using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorialer : MonoBehaviour
{
    private float currentY;
    private float goToY;
    private bool up;
    [SerializeField] private Vector3 NexPosition;
    TextMeshProUGUI text1;
    TextMeshProUGUI text2;
    TextMeshProUGUI text3;
    private bool change;
    private TutorialerState state;
    void Awake()
    {
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != "Player") return;
        if (!change) return;
        change = false;
        switch (state)
        {
            case TutorialerState.State1:
                text1.text = "Bien Hecho! Sabes Moverte! Tal vez seas por fin el heroe que busco, el resto o terminaban carbonizados o basura espacial...";
                text2.text = "En fin espero no tener que buscar más. Si pulsas F puedes hacer un dash, lo necesitaras para este tramo.";
                text2.color = Color.white;
                transform.position = new Vector3(4.14f, 8.26f, 30.98f);
                currentY = transform.position.y;
                goToY = currentY + 1;
                state = TutorialerState.State2;
                break;
            case TutorialerState.State2:
                text1.text = "Bien Ahora que sabes moverte pulsa I para entrar en tu control de arma. Allí podrás configurar tu arma con distintos menus.";
                text3.text = "Y pulsando Q y E puedes cambiar de configuraciones a tiempo real y de Menus de configuracion en interfaz. Prueba a familiarizarte y retoca cosas!";
                text2.text = "Hay dos tipos de configuracion. Los porcentajes de: Power, Accuracy y Frequency; y por otra parte el sistema de puntos de PowerBullets.";
                text2.color = Color.white;
                transform.position = new Vector3(0.39f, 7f, 66.03f);
                currentY = transform.position.y;
                goToY = currentY + 1;
                state = TutorialerState.State3;
                break;
            case TutorialerState.State3:
                text1.text = "Prueba con estos maniquís las diferentes configuraciones";
                text2.text = "Vamos a ver como te mueves!";
                text3.text = "Ven a la siguiente plataforma cuando creas que estás listo!";
                text2.color = Color.white;
                state = TutorialerState.State4;
                break;
            case TutorialerState.State4:
                text1.text = "";
                text2.text = "";
                text3.text = "";
                text2.color = Color.white;
                state = TutorialerState.State5;
                break;
            default:
                break;
        }
        change = true;
    }
}
public enum TutorialerState
{
    State1, State2, State3, State4, State5
}