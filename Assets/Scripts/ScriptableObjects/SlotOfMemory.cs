using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu]
public class SlotOfMemory : ScriptableObject
{
    //      CONSTANTS
    [NonSerialized]
    public const float MaxDispersionValue = 1f;


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

    public int MaxNumOfBulletsPerBurst = 20;
    [NonSerialized]
    public int CurrentNumOfBulletsPerBurst;


    //      Final Variables
    public float DamageBaseWeapon;
    [NonSerialized]
    public float CurrentDamageWeapon;
    [NonSerialized]
    public float CurrentForce;

}
