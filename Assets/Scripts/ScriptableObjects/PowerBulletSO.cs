using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PowerBulletSO", menuName = "Power Bullets")]
public class PowerBulletSO : ScriptableObject
{
    public int ScaleInvestment;
    public string Name;
    public string Description;
    public float InvestmentValue;
    public float currentInvestmentValue;
    public int Points;
    public PowerBulletID id;
    public IncreaseType type;
}
public enum PowerBulletID
{
    STUNE, EXPAND, DOUBLE_FORCE, PIERCING, MULTIPLIER, BOUNCING, BOUCING_SURFACE, FLAME, HEALTH_STEALTH, SHIELD_STEALTH, EXPLOSIVE, CRITICAL, TERRIFIER, TIMESLOW, CRAZYFIER
}
public enum IncreaseType
{
    INVESTMENT, PROBABILITY
}