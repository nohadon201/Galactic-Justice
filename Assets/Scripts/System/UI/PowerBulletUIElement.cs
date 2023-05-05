using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerBulletUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerInfo pi;
    private PowerBulletSO powerBulletInfo;
    private Button Up;
    private Button Down;
    private Texture textureSelected;
    private Texture textureUnSelected;
    private Transform Description,Name,Points;
    private TextMeshProUGUI MultiplierText, PointsText;
    void Awake()
    {
        Up = transform.GetChild(2).gameObject.GetComponent<Button>();
        Down = transform.GetChild(3).gameObject.GetComponent<Button>();

        textureSelected = Resources.Load<Texture>("UI/Menu/PowerSelected");
        textureUnSelected = Resources.Load<Texture>("UI/Menu/PowerNotSelected");
    }
    public void SetValue(PowerBulletSO powerBullet, PlayerInfo playerInfo)
    {

        powerBulletInfo = powerBullet;
        
        pi = playerInfo;

        Name = transform.GetChild(1);
        Name.gameObject.GetComponent<TextMeshProUGUI>().text = powerBullet.Name;

        PointsText = transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>();
        MultiplierText = transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>();

        PointsText.text = powerBullet.Points + "";

        Description = transform.GetChild(6);
        Description.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = powerBullet.Description;
        Description.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "COST: ("+powerBullet.ScaleInvestment+" x1)    ("+ (powerBullet.ScaleInvestment * 2) + " x2)    (" + (powerBullet.ScaleInvestment * 4) + " x3)";
        
        Up.onClick.AddListener(() =>
        {
            if(playerInfo.Points-1 >= 0 && (powerBulletInfo.Points + 1 <= powerBulletInfo.ScaleInvestment * 4))
            {
                pi.Points--;
                powerBulletInfo.Points = powerBulletInfo.Points + 1;
                if(powerBulletInfo.Points == powerBulletInfo.ScaleInvestment * 4)
                {
                    MultiplierText.text = "x3";
                    MultiplierText.color = Color.red;
                }
                else if(powerBulletInfo.Points >= powerBulletInfo.ScaleInvestment * 2)
                {
                    MultiplierText.text = "x2";
                    MultiplierText.color = Color.yellow;
                }
                else if (powerBulletInfo.Points >= powerBulletInfo.ScaleInvestment)
                {
                    MultiplierText.text = "x1";
                    MultiplierText.color = Color.green;
                }
                else
                {
                    MultiplierText.text = "";
                }
                PointsText.text = powerBullet.Points + "";
            }
        });
        Down.onClick.AddListener(() =>
        {
            if (powerBullet.Points > 0)
            {
                pi.Points++;
                powerBulletInfo.Points = powerBulletInfo.Points - 1 < 0 ? powerBulletInfo.Points : powerBulletInfo.Points - 1;
                if (powerBulletInfo.Points == powerBulletInfo.ScaleInvestment * 4)
                {
                    MultiplierText.text = "x3";
                    MultiplierText.color = Color.red;
                }
                else if (powerBulletInfo.Points >= powerBulletInfo.ScaleInvestment * 2)
                {
                    MultiplierText.text = "x2";
                    MultiplierText.color = Color.yellow;
                }
                else if (powerBulletInfo.Points >= powerBulletInfo.ScaleInvestment)
                {
                    MultiplierText.text = "x1";
                    MultiplierText.color = Color.green;
                }
                else
                {
                    MultiplierText.text = "";
                }
                PointsText.text = powerBullet.Points + "";
            }
        });

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int a = 0; a < transform.childCount; a++)
        {
            TextMeshProUGUI TMP = transform.GetChild(a).gameObject.GetComponent<TextMeshProUGUI>();

            if (TMP != null && TMP.color == Color.white) TMP.color = Color.black;
        }
        transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = textureSelected;
        Description.gameObject.SetActive(true);
        Description.transform.parent = transform.parent;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        for (int a = 0; a < transform.childCount; a++)
        {
            TextMeshProUGUI TMP = transform.GetChild(a).gameObject.GetComponent<TextMeshProUGUI>();

            if (TMP != null && TMP.color == Color.black) TMP.color = Color.white;
        }
        transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = textureUnSelected;
        Description.gameObject.SetActive(false);
        Description.transform.parent = transform;
    }
}
