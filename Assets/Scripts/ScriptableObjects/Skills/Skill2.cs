using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Skill2 : Skills
{
    public override IEnumerator SkillCoroutine(PlayerInfo playerInfo, GameObject Player)
    {
        float health = playerInfo.playersMaxHealth;
        playerInfo.playersMaxHealth += playerInfo.playersMaxHealth * 0.25f;
        playerInfo.playersCurrentHealth = playerInfo.playersMaxHealth;
        yield return new WaitForSeconds(60);
        playerInfo.playersMaxHealth -= health * 0.25f;
    }

    public override void initValues()
    {
        Name = Name == null || Name == "" ? "I Am Healthy" : Name;
        Description = Description == null || Description == "" ? "During 1 minute the player will have 25% more of max health and will recover all current health." : Description;
    }
}