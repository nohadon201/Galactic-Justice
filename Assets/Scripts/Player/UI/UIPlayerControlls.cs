using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerControlls : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private PlayerInfo playerInformation;
    Image sliderOfAmmunition;
    Image sliderOfShield;
    Image sliderOfHealth;

    void Awake()
    {
        //Set active for default the UI objects
        GameObject crosshair = transform.GetChild(0).gameObject;
        GameObject Ammunition = transform.GetChild(1).gameObject;
        GameObject ShieldBar = transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
        GameObject HeralthdBar = transform.GetChild(2).GetChild(1).GetChild(0).gameObject;
        GameObject menuCustomizationParent = transform.GetChild(3).gameObject;

        if (menuCustomizationParent.activeSelf) menuCustomizationParent.SetActive(false);

        if (!Ammunition.activeSelf) Ammunition.SetActive(true);

        if (!transform.GetChild(2).gameObject.activeSelf) transform.GetChild(2).gameObject.SetActive(true);

        if(!crosshair.activeSelf) crosshair.SetActive(true);

        sliderOfAmmunition = Ammunition.GetComponent<Image>();
        sliderOfShield = ShieldBar.GetComponent<Image>();
        sliderOfHealth = HeralthdBar.GetComponent<Image>();
    }
    public void setValues(PlayerInfo info, PlayerWeapon weapon)
    {
        playerWeapon = weapon;
        playerInformation = info;
        StartCoroutine(UpdateValues());
    }
    public IEnumerator UpdateValues()
    {
        while(true)
        {
            sliderOfHealth.fillAmount = playerInformation.playersCurrentHealth / playerInformation.playersMaxHealth;
            sliderOfShield.fillAmount = playerInformation.playersCurrentShield / playerInformation.playersMaxShield;
            sliderOfAmmunition.fillAmount = playerWeapon.CurrentConfiguration.CurrentAmmunition / playerWeapon.CurrentConfiguration.MaxAmmunition;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
