using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerControlls;

public class UIPlayerControlls : MonoBehaviour
{
    private GameObject prefabConfiguration, player, powerBulletPrefab;
    [SerializeField]
    private PlayerWeapon playerWeapon;
    [SerializeField]
    private PlayerInfo playerInformation;
    private Texture slotSelected;
    private Texture slotNotSelected;
    Image sliderOfAmmunition;
    Image sliderOfShield;
    Image sliderOfHealth;
    [SerializeField]
    private List<ConfigurationUI> configurationUI = new List<ConfigurationUI>();
    private int MenuDisplayed;
    private TextMeshProUGUI points;
    void Awake()
    {

        //Set active for default the UI objects
        MenuDisplayed = 1;
        points = transform.GetChild(4).GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();   
        GameObject crosshair = transform.GetChild(0).gameObject;
        GameObject Ammunition = transform.GetChild(1).gameObject;
        GameObject ShieldBar = transform.GetChild(2).GetChild(0).GetChild(1).gameObject;
        GameObject HeralthdBar = transform.GetChild(2).GetChild(1).GetChild(1).gameObject;
        GameObject menuCustomizationParent = transform.GetChild(4).gameObject;
        
        prefabConfiguration = Resources.Load<GameObject>("Prefabs/Player/SlotOfMemoryPrefab");
        powerBulletPrefab = Resources.Load<GameObject>("Prefabs/Player/PowerBulletPrefab");

        if (menuCustomizationParent.activeSelf) menuCustomizationParent.SetActive(false);

        if (!Ammunition.activeSelf) Ammunition.SetActive(true);

        if (!transform.GetChild(2).gameObject.activeSelf) transform.GetChild(2).gameObject.SetActive(true);

        if(!crosshair.activeSelf) crosshair.SetActive(true);

        sliderOfAmmunition = Ammunition.GetComponent<Image>();
        sliderOfShield = ShieldBar.GetComponent<Image>();
        sliderOfHealth = HeralthdBar.GetComponent<Image>();

        slotNotSelected = Resources.Load<Texture>("UI/Menu/NotSelected");
        slotSelected = Resources.Load<Texture>("UI/Menu/Selected");

    }

    public void setValues(GameObject Player)
    {
        playerWeapon = Player.GetComponent<PlayerWeapon>();
        playerInformation = Player.GetComponent<PlayerControlls>().OwnInfo;
        this.player = Player;
        PlayerControlls pc = player.GetComponent<PlayerControlls>();
        pc.displayInterfaceDelegator += displayMenu;
        pc.changeSlotDelegator += ChangeSlotInRealTime;
        pc.goToNextInterfaceDelegator += GoToNextInterface;
        int ver = 0;
        int hor = 0;
        GetComponent<WinPointsListener>().Response.AddListener(pc.WinPoints);
        foreach (PowerBulletSO powerBullet in pc.GetComponent<PowerBullets>().powerBullets)
        {
            GameObject pbi = Instantiate(powerBulletPrefab);
            pbi.transform.parent = transform.GetChild(4).GetChild(2);
            pbi.transform.localPosition = new Vector3(0 + (350f * hor), 0 + (-200f * ver), 0);
            pbi.GetComponent<PowerBulletUIElement>().SetValue(powerBullet, pc.OwnInfo);
            if(ver == 2)
            {
                ver = 0;
                hor++;
            }
            else
            {
                ver++;
            }
        }
        int a = 0;
        foreach(SlotOfMemory som in playerWeapon.WeaponConfigurations)
        {
            GameObject somi = Instantiate(prefabConfiguration);
            configurationUI.Add(new ConfigurationUI(somi, som, 
                somi.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>(), 
                somi.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>(), 
                somi.transform.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>()
                )
                );
            somi.transform.parent = transform.GetChild(3);
            somi.transform.localPosition = new Vector3(0 + (150f * a), 0, 0);
            a++;
        }
        int e = 0;
        foreach (ConfigurationUI cUI in configurationUI)
        {
            if (e == playerWeapon.IndexCurrentConfiguration)
            {
                cUI.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = slotSelected;
                cUI.Freq.color = Color.black;
                cUI.Acc.color = Color.black;
                cUI.Power.color = Color.black;
            }
            else
            {
                cUI.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = slotNotSelected;
                cUI.Freq.color = Color.white;
                cUI.Acc.color = Color.white;
                cUI.Power.color = Color.white;
            }
            cUI.Power.text = MathF.Truncate(cUI.slotOfMemory.Power * 100f) + "%";
            cUI.Acc.text = MathF.Truncate(cUI.slotOfMemory.Accuracy * 100) + "%";
            cUI.Freq.text = MathF.Truncate(cUI.slotOfMemory.Frequency * 100) + "%";
            e++;
        }
        StartCoroutine(UpdateValues());
    }

    public IEnumerator UpdateValues()
    {
        while(true)
        {
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                sliderOfHealth.fillAmount = playerInformation.playersCurrentHealth / playerInformation.playersMaxHealth;
                sliderOfShield.fillAmount = playerInformation.playersCurrentShield / playerInformation.playersMaxShield;
                sliderOfAmmunition.fillAmount = playerWeapon.CurrentConfiguration.CurrentAmmunition / playerWeapon.CurrentConfiguration.MaxAmmunition;
            }
            else
            {
                transform.GetChild(4).GetChild(1).GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MathF.Truncate(playerWeapon.CurrentConfiguration.Power * 100f) + "%";
                transform.GetChild(4).GetChild(1).GetChild(1).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MathF.Truncate(playerWeapon.CurrentConfiguration.Accuracy * 100f)  + "%";
                transform.GetChild(4).GetChild(1).GetChild(2).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MathF.Truncate(playerWeapon.CurrentConfiguration.Frequency * 100f) + "%";
                points.text = "Current points: "+playerInformation.Points;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void displayMenu()
    {
        
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(true);
            
            transform.GetChild(4).GetChild(1).GetChild(0).GetChild(2).gameObject.GetComponent<Slider>().value = playerWeapon.CurrentConfiguration.Power;
            transform.GetChild(4).GetChild(1).GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MathF.Truncate(playerWeapon.CurrentConfiguration.Power * 100f)+"%";
            
            transform.GetChild(4).GetChild(1).GetChild(1).GetChild(2).gameObject.GetComponent<Slider>().value = playerWeapon.CurrentConfiguration.Accuracy;
            transform.GetChild(4).GetChild(1).GetChild(1).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MathF.Truncate(playerWeapon.CurrentConfiguration.Accuracy * 100f) + "%";
            
            transform.GetChild(4).GetChild(1).GetChild(2).GetChild(2).gameObject.GetComponent<Slider>().value = playerWeapon.CurrentConfiguration.Frequency;
            transform.GetChild(4).GetChild(1).GetChild(2).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MathF.Truncate(playerWeapon.CurrentConfiguration.Frequency * 100f)  + "%";

        }
        else
        {
            playerWeapon.CurrentConfiguration.LoadConfigurationOfWeapon();
            ChangeSlotInRealTime();
            Cursor.lockState = CursorLockMode.Locked;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.SetActive(false);
        }
        
    }
    private void ChangeSlotInRealTime()
    {
        int a = 0;
        foreach (ConfigurationUI cUI in configurationUI)
        {
            if (a == playerWeapon.IndexCurrentConfiguration)
            {
                cUI.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = slotSelected;
                cUI.Freq.color = Color.black;
                cUI.Acc.color = Color.black;
                cUI.Power.color = Color.black;
            }
            else
            {
                cUI.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = slotNotSelected;
                cUI.Freq.color = Color.white;
                cUI.Acc.color = Color.white;
                cUI.Power.color = Color.white;
            }
            cUI.Power.text = MathF.Truncate(cUI.slotOfMemory.Power * 100f) + "%";
            cUI.Acc.text = MathF.Truncate(cUI.slotOfMemory.Accuracy * 100) + "%";
            cUI.Freq.text = MathF.Truncate(cUI.slotOfMemory.Frequency * 100) + "%";
            a++;
        }
    }
    public void ChangeAccuracy(float f)
    {
        playerWeapon.CurrentConfiguration.Accuracy = f;
    }
    public void ChangePower(float f)
    {
        playerWeapon.CurrentConfiguration.Power = f;
    }
    public void ChangeFrequency(float f)
    {
        playerWeapon.CurrentConfiguration.Frequency = f;
    }
    public void GoToNextInterface(bool b)
    {
        transform.GetChild(4).GetChild(MenuDisplayed).gameObject.SetActive(false);
        if(b) {
            MenuDisplayed = MenuDisplayed + 1 == transform.GetChild(4).childCount ? 1 : MenuDisplayed + 1;
        }
        else
        {
            MenuDisplayed = MenuDisplayed - 1 == 0 ? transform.GetChild(4).childCount - 1 : MenuDisplayed - 1;
        }
        transform.GetChild(4).GetChild(MenuDisplayed).gameObject.SetActive(true);
    }
}
[Serializable]
public struct ConfigurationUI
{
    public GameObject gameObject;
    public SlotOfMemory slotOfMemory;
    public TextMeshProUGUI Freq;
    public TextMeshProUGUI Power;
    public TextMeshProUGUI Acc;
    public ConfigurationUI(GameObject go, SlotOfMemory som, TextMeshProUGUI Pow, TextMeshProUGUI Acc, TextMeshProUGUI Freq)
    {
        gameObject = go;
        slotOfMemory = som;
        this.Freq = Freq;
        this.Power = Pow;
        this.Acc = Acc;
    }
}