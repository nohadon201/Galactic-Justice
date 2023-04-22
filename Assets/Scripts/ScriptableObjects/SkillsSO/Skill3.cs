using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill3", menuName = "Player/Skills/Skill3")]
public class Skill3 : Skills
{
    public override IEnumerator SkillCoroutine(PlayerInfo playerInfo, GameObject Player)
    {
        float shield = playerInfo.playersMaxShield;
        playerInfo.playersMaxShield += playerInfo.playersMaxShield * 0.25f;
        playerInfo.playersCurrentShield = playerInfo.playersMaxShield;
        yield return new WaitForSeconds(60);
        if(playerInfo.playersCurrentShield > shield)
        {
            playerInfo.playersMaxShield -= shield * 0.25f;
            playerInfo.playersCurrentShield = playerInfo.playersMaxShield;
        }
        else
        {
            playerInfo.playersMaxShield -= shield * 0.25f;
        }
    }

    public override void initValues()
    {
        Name = Name == null || Name == "" ? "Armor Man" : Name;
        Description = Description == null || Description == "" ? "During 1 minute the player will have 25% more of max Shield and will recover all current shield." : Description;
    }
}
