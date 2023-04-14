using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Skill5", menuName = "Player/Skills/Skill5")]
public class Skill5 : Skills
{
    public override void initValues()
    {
        Name = "Skill 5";
        Description = "";

    }

    public override IEnumerator SkillCoroutine(PlayerInfo PlayerInfo, GameObject Player)
    {
        yield return null;  
    }
}
