using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Player Information", menuName = "Player/Player Information")]
public class PlayerInfo : ScriptableObject
{
    //      Controls Variables
    [Header("Player Information")]
    public int TotalPoints;
    public int Points;
    public float playerVelocity;

    public float playersMaxHealth;
    public float playersCurrentHealth;

    public float playersMaxShield;
    public float playersCurrentShield;

    public float RegenerationShieldValue;

    public float Sensibility;

    public List<SlotOfMemory> MemorySlots;

    public List<Skills> abilities;

    public void DefaultValues(bool Host)
    {
        if (abilities.Count == 0)
        {
            abilities.Add(Resources.Load<Skills>("Abilities/Skill1"));
            abilities.Add(Resources.Load<Skills>("Abilities/Skill2"));
            abilities.Add(Resources.Load<Skills>("Abilities/Skill3"));
            abilities.Add(Resources.Load<Skills>("Abilities/Skill4"));
        }
        if (MemorySlots.Count == 0 && Host)
        {
            MemorySlots.AddRange(Resources.LoadAll<SlotOfMemory>("Player/Host/SlotOfMemory/"));
            
        }
        else if (MemorySlots.Count == 0)
        {
            MemorySlots.AddRange(Resources.LoadAll<SlotOfMemory>("Player/Client/SlotOfMemory/"));
        }

        playerVelocity = playerVelocity == 0 ? 7f : playerVelocity;

        playersMaxHealth = playersMaxHealth == 0 ? 100 : playersMaxHealth;
        playersMaxShield = playersMaxShield == 0 ? 300 : playersMaxShield;

        Sensibility = Sensibility == 0 ? 0.3f : Sensibility;

        playersCurrentHealth = playersMaxHealth;
        playersCurrentShield = playersMaxShield;

        RegenerationShieldValue = RegenerationShieldValue == 0 ? 0.3f : RegenerationShieldValue;
        Points = 0;
    }
}
