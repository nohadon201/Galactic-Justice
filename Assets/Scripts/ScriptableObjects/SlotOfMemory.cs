using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "Slot", menuName = "Player/Slot of Memory")]
public class SlotOfMemory : ScriptableObject
{
    //      WEAPON CONFIGURATION

    [Header("")]
    [Header("Weapon Configuration")]
    [DefaultValue(1)]
    [Range(0.001f, 1f)]
    public float Power;

    [DefaultValue(1)]
    [Range(0.001f, 1f)]
    public float Accuracy;

    [DefaultValue(1)]
    [Range(0.001f, 1f)]
    public float Frequency;

    [Header("")]
    [Header("Base Stats")]


    //      Power Variables

    public float MaxRange;

    public float MaxForce;

    //      Accuracy Variables
    [NonSerialized]
    public Vector3[] MaxDispersion = new Vector3[8];
    [NonSerialized]
    public Vector3[] CurrentDispersion = new Vector3[8];

    //      Frequency Variables

    public int MaxNumOfBulletsPerBurst;
    [NonSerialized] public int CurrentNumOfBulletsPerBurst;
    public float MaxCooldownBetweenBullets;
    [NonSerialized] public float CurrentCooldownBetweenBullets;

    //      Final Variables
    public float MaxAmmunition;
    [NonSerialized] public float CurrentAmmunition;
    public float RegenerationValueAmmunition;
    public float MaxWasteOfAmmunitionValue;
    [NonSerialized] public float CurrentWasteOfAmmunitionPerBullet;

    public float DamageBaseWeapon;
    [NonSerialized] public float CurrentDamageWeapon;
    [NonSerialized] public float CurrentForce;



    public void defaultValues()
    {
        MaxRange = MaxRange == 0 ? 50 : MaxRange;
        MaxForce = MaxForce == 0 ? 200 : MaxForce;
        Power = Power == 0 ? 1f : Power;
        Accuracy = Accuracy == 0 ? 1f : Accuracy;
        Frequency = Frequency == 0 ? 1f : Frequency;

        DamageBaseWeapon = DamageBaseWeapon == 0 ? 100 : DamageBaseWeapon;
        MaxNumOfBulletsPerBurst = MaxNumOfBulletsPerBurst == 0 ? 20 : MaxNumOfBulletsPerBurst;
        MaxAmmunition = MaxAmmunition == 0 ? 100f : MaxAmmunition;
        MaxWasteOfAmmunitionValue = MaxWasteOfAmmunitionValue == 0 ? 100f : MaxWasteOfAmmunitionValue;
        CurrentAmmunition = MaxAmmunition;
        MaxCooldownBetweenBullets = MaxCooldownBetweenBullets == 0 ? 5f : MaxCooldownBetweenBullets;
        RegenerationValueAmmunition = RegenerationValueAmmunition == 0 ? 0.1f : RegenerationValueAmmunition;
    }
    public void DispersionValues()
    {
        float sqr2 = Mathf.Sqrt(2);
        MaxDispersion[0] = new Vector3(1, 0, 0);

        MaxDispersion[1] = new Vector3(sqr2 / 2, sqr2 / 2, 0);

        MaxDispersion[2] = new Vector3(0, 1, 0);

        MaxDispersion[3] = new Vector3(-sqr2 / 2, sqr2 / 2, 0);

        MaxDispersion[4] = new Vector3(-1, 0, 0);

        MaxDispersion[5] = new Vector3(-sqr2 / 2, -sqr2 / 2, 0);

        MaxDispersion[6] = new Vector3(0, -1, 0);

        MaxDispersion[7] = new Vector3(sqr2 / 2, -sqr2 / 2, 0);
    }

    //      LoadConfigurationOfWeapon will be executed every time the player change the configuration.
    public void LoadConfigurationOfWeapon()
    {

        CurrentNumOfBulletsPerBurst = Frequency == 1 ? 1 : Mathf.RoundToInt(MaxNumOfBulletsPerBurst * (1 - Frequency));

        CurrentDamageWeapon = Accuracy == 1 ?
            (DamageBaseWeapon * Power) / CurrentNumOfBulletsPerBurst
            :
            ((DamageBaseWeapon * Power) / 8) / CurrentNumOfBulletsPerBurst;

        
        CurrentForce = Accuracy == 1 ?
            (MaxForce * Power) / CurrentNumOfBulletsPerBurst
            :
            ((MaxForce * Power) / 8) / CurrentNumOfBulletsPerBurst;

        
        CurrentWasteOfAmmunitionPerBullet = Accuracy == 1 ?
            (MaxWasteOfAmmunitionValue * Power) / CurrentNumOfBulletsPerBurst
            :
            ((MaxWasteOfAmmunitionValue * Power) / 8) / CurrentNumOfBulletsPerBurst;

        
        CurrentCooldownBetweenBullets = Accuracy == 1 ?
            (MaxCooldownBetweenBullets * Power) / CurrentNumOfBulletsPerBurst
            :
            ((MaxCooldownBetweenBullets * Power) / 8) / CurrentNumOfBulletsPerBurst;


        for (int a = 0; a < MaxDispersion.Length; a++)
        {
            CurrentDispersion[a] = MaxDispersion[a] * (1 - Accuracy);
        }
    }

    public void setRangeVariables()
    {
        Power = Mathf.Clamp(Power, 0.01f, 1);
        Accuracy = Mathf.Clamp(Accuracy, 0.01f, 1);
        Frequency = Mathf.Clamp(Frequency, 0.01f, 1);
    }


}
