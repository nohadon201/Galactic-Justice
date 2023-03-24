using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class PlayerInfo : ScriptableObject
{
    //      Controls Variables
    [Header("Player Information")]

    
    public float playerVelocity;

    public float playersMaxHealth;
    public float playersCurrentHealth;

    public float playersMaxShield;
    public float playersCurrentShield;

    public float RegenerationShieldValue;

    public float Sensibility;

    public List<SlotOfMemory> MemorySlots;

    

    public void DefaultValues()
    {
        playerVelocity = playerVelocity == 0 ? 3f : playerVelocity;

        playersMaxHealth = playersMaxHealth == 0 ? 100 : playersMaxHealth;
        playersMaxShield = playersMaxShield == 0 ? 300 : playersMaxShield;  

        Sensibility = Sensibility == 0 ? 0.3f : Sensibility;

        playersCurrentHealth = playersMaxHealth;
        playersCurrentShield = playersMaxShield;

        RegenerationShieldValue = RegenerationShieldValue == 0 ? 0.3f : RegenerationShieldValue;

    }
}
