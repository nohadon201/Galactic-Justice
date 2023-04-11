using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
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
    protected Camera camera;

    //######################## CONFIGURATIONS ########################

    public SlotOfMemory CurrentConfiguration;
    [SerializeField]
    public List<SlotOfMemory> WeaponConfigurations;

    public int IndexCurrentConfiguration;
    
    //      Functions that executes only at Start
    void Awake()
    {
        powerBullets = GetComponent<PowerBullets>();
        if(camera == null) camera = Object.FindObjectOfType<Camera>();
        IndexCurrentConfiguration = 0;
        CurrentConfiguration = WeaponConfigurations[IndexCurrentConfiguration];
        for(int i = 0; i < WeaponConfigurations.Count; i++) { 
            
            WeaponConfigurations[i].defaultValues(); 
            
            WeaponConfigurations[i].DispersionValues(); 
            
            WeaponConfigurations[i].LoadConfigurationOfWeapon(); 
        
        }
        Shooting = false;
    }
    private void Start()
    {
        //Debug.Log(CurrentConfiguration.ToString());
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
            return CurrentConfiguration.CurrentAmmunition - CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet>=0;
        }
        else
        {
            return CurrentConfiguration.CurrentAmmunition - (CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet * 8 ) >= 0;
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
        if(CurrentConfiguration.CurrentAmmunition <= 0 || !CanShoot())
        {
            this.Shooting = false;  
        }
    }
    
    public void CalculateAccuracy()
    {
        if (CurrentConfiguration.Accuracy == 1)
        {
            RayCastTo(transform.position, camera.transform.forward, false);
        }
        else
        {
            for (int a = 0; a < CurrentConfiguration.CurrentDispersion.Length; a++)
            {
                RayCastTo(transform.position, CurrentConfiguration.CurrentDispersion[a] + camera.transform.forward, false);
            }
        }
    }
    
    public void RayCastTo(Vector3 origin, Vector3 v, bool byPowerBullet)
    {
        if(!byPowerBullet)
            CurrentConfiguration.CurrentAmmunition -= CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet;
        RaycastHit hit;
        if (Physics.Raycast(origin, v, out hit, CurrentConfiguration.MaxRange * CurrentConfiguration.Power))
        {
            powerBullets.execute(hit, byPowerBullet);  
            Debug.DrawLine(origin, hit.point, Color.red, 3f);
            if(hit.transform.tag.Contains("Physics"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce, hit.point);
                //Uncomment this only for debug
                //hit.transform.gameObject.GetComponent<aaaScript>().cosa2();
            }else if (hit.transform.tag.Contains("Enemy"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce * 10, hit.point);
            }
            
        }
    }
}
