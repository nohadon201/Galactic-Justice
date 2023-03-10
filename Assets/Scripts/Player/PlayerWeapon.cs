using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    //######################## STATES ########################
    
    public bool Shooting;
    public bool Regeneration;

    //######################## CAMERA ########################

    [Header("")]
    [Header("Others")]
    [SerializeField]
    private Camera camera;

    //######################## CONFIGURATIONS ########################

    [SerializeField]
    public SlotOfMemory CurrentConfiguration;
    [SerializeField]
    public List<SlotOfMemory> WeaponConfigurations; 

    
    //      Functions that executes only at Start
    void Awake()
    {
        
        if(camera == null) camera = Object.FindObjectOfType<Camera>();
        CurrentConfiguration.defaultValues();
        CurrentConfiguration.DispersionValues();
        CurrentConfiguration.LoadConfigurationOfWeapon();
        Shooting = false;
    }
    private void Start()
    {
        Debug.Log(CurrentConfiguration.ToString());
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
    public void RegenerateAmmunition()
    {
        Debug.Log("Regenerating: " + CurrentConfiguration.CurrentAmmunition);
        CurrentConfiguration.CurrentAmmunition += CurrentConfiguration.RegenerationValueAmmunition;
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
        Debug.Log("aaaaaaaaaaaa");
        if (CurrentConfiguration.Accuracy == 1)
        {
            RayCastTo(camera.transform.forward);
        }
        else
        {
            for (int a = 0; a < CurrentConfiguration.CurrentDispersion.Length; a++)
            {
                RayCastTo(CurrentConfiguration.CurrentDispersion[a] + camera.transform.forward);
            }
        }
    }
    private void RayCastTo(Vector3 v)
    {
        CurrentConfiguration.CurrentAmmunition -= CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet;
        //if (CurrentConfiguration.CurrentAmmunition <= 0) Debug.Log("Tonto que no tienes más");
        //else Debug.Log("aaaaaaaa");
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, v, out hit, CurrentConfiguration.MaxRange * CurrentConfiguration.Power))
        {
            Debug.DrawLine(camera.transform.position, hit.point, Color.red, 3f);
            if(hit.transform.tag.Contains("Physics"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * CurrentConfiguration.CurrentForce, hit.point);
            }
            
        }
    }
}
