using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : NetworkBehaviour
{
    //######################## PowerBullets ########################
    PowerBullets powerBullets;
    //######################## STATES ########################

    public bool Shooting;
    public bool Regeneration;

    //######################## CAMERA ########################

    [Header("")]
    [Header("Others")]
    [SerializeField]
    public Camera camera;

    //######################## CONFIGURATIONS ########################

    public SlotOfMemory CurrentConfiguration;
    public List<SlotOfMemory> WeaponConfigurations;

    public int IndexCurrentConfiguration;

    //      Functions that executes only at Start
    private void Start()
    {
        powerBullets = GetComponent<PowerBullets>();
        if(IsServer && IsOwner)
            WeaponConfigurations = Resources.LoadAll<SlotOfMemory>("Player/Host/SlotOfMemory").ToList();
        else
            WeaponConfigurations = Resources.LoadAll<SlotOfMemory>("Player/Client/SlotOfMemory").ToList();
        IndexCurrentConfiguration = 0;
        CurrentConfiguration = WeaponConfigurations[IndexCurrentConfiguration];
        for (int i = 0; i < WeaponConfigurations.Count; i++)
        {

            WeaponConfigurations[i].defaultValues();

            WeaponConfigurations[i].DispersionValues();

            WeaponConfigurations[i].LoadConfigurationOfWeapon();

        }
        Shooting = false;
    }
    public void Shoot()
    {
        if (CurrentConfiguration.CurrentAmmunition > 0)
        {
            CalculateAccuracy();
            CheckIfAmmunition();
        }
    }
    public bool CanShoot()
    {
        if (CurrentConfiguration.Accuracy == 1)
        {
            return CurrentConfiguration.CurrentAmmunition - CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet >= 0;
        }
        else
        {
            return CurrentConfiguration.CurrentAmmunition - (CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet * 8) >= 0;
        }
    }

    public void RegenerateAmmunition(float RegenerationValue)
    {
        Debug.Log("Regenerating: " + CurrentConfiguration.CurrentAmmunition);
        CurrentConfiguration.CurrentAmmunition += RegenerationValue;
    }
    public bool AmmunitionEmpty()
    {
        return CurrentConfiguration.CurrentAmmunition >= CurrentConfiguration.MaxAmmunition;
    }
    public void CheckIfAmmunition()
    {
        if (CurrentConfiguration.CurrentAmmunition <= 0 || !CanShoot())
        {
            this.Shooting = false;
        }
    }

    public void CalculateAccuracy()
    {
        if (CurrentConfiguration.Accuracy == 1)
        {
            if (IsClient)
            {
                RayCastToServerRpc(transform.position, camera.gameObject.transform.forward, false);
            }
            else
            {
                RayCastTo(transform.position, camera.transform.forward, false);
            }

        }
        else
        {
            for (int a = 0; a < CurrentConfiguration.CurrentDispersion.Length; a++)
            {
                if (IsClient)
                {
                    RayCastToServerRpc(transform.position, CurrentConfiguration.CurrentDispersion[a] + camera.transform.forward, false);
                }
                else
                {
                    RayCastTo(transform.position, CurrentConfiguration.CurrentDispersion[a] + camera.transform.forward, false);
                }
            }

        }
    }


    public void RayCastTo(Vector3 origin, Vector3 v, bool byPowerBullet)
    {
        if (!byPowerBullet)
            CurrentConfiguration.CurrentAmmunition -= CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet;
        
        RaycastHit hit;
        
        if (Physics.Raycast(origin, v, out hit, CurrentConfiguration.MaxRange * CurrentConfiguration.Power))
        {
            powerBullets.execute(hit, byPowerBullet);
            Debug.DrawLine(origin, hit.point, Color.red, 3f);
            if (hit.transform.tag.Contains("Physics"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce, hit.point);
            }
            else if (hit.transform.tag.Contains("Enemy"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce * 10, hit.point);
            }

        }
    }

    [ServerRpc]
    public void RayCastToServerRpc(Vector3 origin, Vector3 v, bool byPowerBullet)
    {
        if (!byPowerBullet)
            RestAmmunitionClientRpc();
        
        RaycastHit hit;
        
        if (Physics.Raycast(origin, v, out hit, CurrentConfiguration.MaxRange * CurrentConfiguration.Power))
        {
            powerBullets.execute(hit, byPowerBullet);
            Debug.DrawLine(origin, hit.point, Color.red, 3f);
            if (hit.transform.tag.Contains("Physics"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce, hit.point);
            }
            else if (hit.transform.tag.Contains("Enemy"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce * 10, hit.point);
            }

        }
    }
    [ClientRpc]
    private void RestAmmunitionClientRpc()
    {
        if (!IsOwner) return;
        CurrentConfiguration.CurrentAmmunition -= CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet;
    }
}
