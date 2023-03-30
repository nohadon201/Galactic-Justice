using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Skill4 : Skills
{
    private bool DamageChange;
    public override IEnumerator SkillCoroutine(PlayerInfo playerInfo, GameObject Player)
    {
        float damage = Player.GetComponent<PlayerWeapon>().CurrentConfiguration.DamageBaseWeapon;
        
        Player.GetComponent<PlayerWeapon>().CurrentConfiguration.DamageBaseWeapon = damage + (damage * 0.25f);
        
        foreach(SlotOfMemory som in Player.GetComponent<PlayerWeapon>().WeaponConfigurations)
        {
            som.DamageBaseWeapon = damage + (damage * 0.25f);
        }
        
        yield return new WaitForSeconds(3);
        
        Player.GetComponent<PlayerWeapon>().CurrentConfiguration.DamageBaseWeapon = damage;
        foreach (SlotOfMemory som in Player.GetComponent<PlayerWeapon>().WeaponConfigurations)
        {
            som.DamageBaseWeapon = damage;
        }
        Debug.Log("Done!");
    }
    private void setDamage(float currentDamage, GameObject Player)
    {
        DamageChange= true;
        while(DamageChange) {
            Player.GetComponent<PlayerWeapon>().CurrentConfiguration.DamageBaseWeapon = currentDamage + (currentDamage * 0.25f);
        }
    }
    public override void initValues()
    {
        Name = Name == null || Name == "" ? "Armor Man" : Name;
        Description = Description == null || Description == "" ? "During 1 minute the player will have 25% more of max Shield and will recover all current shield." : Description;
    }
}
