using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skills : ScriptableObject
{
    public string Name;
    public string Description;
    public Image Image;
    public abstract void initValues();
    public abstract IEnumerator SkillCoroutine(PlayerInfo PlayerInfo, GameObject Player);
    
}
