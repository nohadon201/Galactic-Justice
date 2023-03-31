using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill1", menuName = "Skills/Skill1")]
public class Skill1 : Skills
{
    public override IEnumerator SkillCoroutine(PlayerInfo playerInfo, GameObject Player)
    {
        float vel = playerInfo.playerVelocity;
        playerInfo.playerVelocity += playerInfo.playerVelocity * 0.25f;
        yield return new WaitForSeconds(60);
        playerInfo.playerVelocity -= vel * 0.25f;
        Debug.Log("DONE! " + playerInfo.playerVelocity);
    }

    public override void initValues()
    {
        Name = Name == null || Name == "" ? "Faster" : Name;
        Description = Description == null || Description == "" ? "This ability makes that during 1 minute the player will have a 25% more velocity." : Description;
    }
}
