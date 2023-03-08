using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    //######################## STATES ########################
    
    public bool Shooting;

    //######################## CAMERA ########################
    
    [Header("")]
    [Header("Others")]
    [SerializeField]
    private Camera camera;

    //######################## CONFIGURATIONS ########################

    [SerializeField] 
    private SlotOfMemory CurrentConfiguration;
    [SerializeField]
    private List<SlotOfMemory> WeaponConfigurations; 

    
    //      Functions that executes only at Start
    void Awake()
    {
        
        if(camera == null) camera = Object.FindObjectOfType<Camera>();
        defaultValues();
        DispersionValues();
        LoadConfigurationOfWeapon();
        
    }
    public void defaultValues()
    {
        CurrentConfiguration.MaxRange = CurrentConfiguration.MaxRange == 0 ? 20 : CurrentConfiguration.MaxRange;
        CurrentConfiguration.MaxForce = CurrentConfiguration.MaxForce == 0 ? 200 : CurrentConfiguration.MaxForce;
        CurrentConfiguration.Power = CurrentConfiguration.Power == 0 ? 1f : CurrentConfiguration.Power;
        CurrentConfiguration.Accuracy = CurrentConfiguration.Accuracy == 0 ? 1f : CurrentConfiguration.Accuracy;
        CurrentConfiguration.Frequency = CurrentConfiguration.Frequency == 0 ? 1f : CurrentConfiguration.Frequency;
        CurrentConfiguration.DamageBaseWeapon = CurrentConfiguration.DamageBaseWeapon == 0 ? 100 : CurrentConfiguration.DamageBaseWeapon;
        Shooting = false;
    }
    public void DispersionValues()
    {
        CurrentConfiguration.MaxDispersion[0] = new Vector3(SlotOfMemory.MaxDispersionValue, 0,0);
        
        CurrentConfiguration.MaxDispersion[1] = new Vector3(Mathf.Sqrt(SlotOfMemory.MaxDispersionValue), Mathf.Sqrt(SlotOfMemory.MaxDispersionValue),0);
        
        CurrentConfiguration.MaxDispersion[2] = new Vector3(0, SlotOfMemory.MaxDispersionValue, 0);
        
        CurrentConfiguration.MaxDispersion[3] = new Vector3(-Mathf.Sqrt(SlotOfMemory.MaxDispersionValue), Mathf.Sqrt(SlotOfMemory.MaxDispersionValue), 0);
        
        CurrentConfiguration.MaxDispersion[4] = new Vector3(-SlotOfMemory.MaxDispersionValue, 0, 0);
        
        CurrentConfiguration.MaxDispersion[5] = new Vector3(-Mathf.Sqrt(SlotOfMemory.MaxDispersionValue), -Mathf.Sqrt(SlotOfMemory.MaxDispersionValue), 0);
        
        CurrentConfiguration.MaxDispersion[6] = new Vector3(0, -SlotOfMemory.MaxDispersionValue, 0);
        
        CurrentConfiguration.MaxDispersion[7] = new Vector3(Mathf.Sqrt(SlotOfMemory.MaxDispersionValue), -Mathf.Sqrt(-SlotOfMemory.MaxDispersionValue), 0);
    }

    //      LoadConfigurationOfWeapon will be executed every time the player change the configuration.
    public void LoadConfigurationOfWeapon()
    {

        CurrentConfiguration.CurrentNumOfBulletsPerBurst = CurrentConfiguration.Frequency == 1 ? 1 : Mathf.RoundToInt(CurrentConfiguration.MaxNumOfBulletsPerBurst * (1- CurrentConfiguration.Frequency));

        CurrentConfiguration.CurrentDamageWeapon = CurrentConfiguration.Accuracy == 1 ? 
            (CurrentConfiguration.DamageBaseWeapon * CurrentConfiguration.Power ) / CurrentConfiguration.CurrentNumOfBulletsPerBurst 
            : 
            ( (CurrentConfiguration.DamageBaseWeapon * CurrentConfiguration.Power ) / 8 ) / CurrentConfiguration.CurrentNumOfBulletsPerBurst;

        CurrentConfiguration.CurrentForce = CurrentConfiguration.Accuracy == 1 ?
            (CurrentConfiguration.MaxForce * CurrentConfiguration.Power) / CurrentConfiguration.CurrentNumOfBulletsPerBurst
            :
            ((CurrentConfiguration.MaxForce * CurrentConfiguration.Power) / 8) / CurrentConfiguration.CurrentNumOfBulletsPerBurst;


        for (int a = 0; a< CurrentConfiguration.MaxDispersion.Length; a++)
        {
            CurrentConfiguration.CurrentDispersion[a] = CurrentConfiguration.MaxDispersion[a] * (1- CurrentConfiguration.Accuracy); 
        }
    }

    public void setRangeVariables()
    {
        CurrentConfiguration.Power = Mathf.Clamp(CurrentConfiguration.Power, 0.01f,1);
        CurrentConfiguration.Accuracy = Mathf.Clamp(CurrentConfiguration.Accuracy, 0.01f, 1);
        CurrentConfiguration.Frequency = Mathf.Clamp(CurrentConfiguration.Frequency, 0.01f, 1);
    }

    public void Shoot()
    {
        CalculateAccuracy();
    }
    public void CalculateAccuracy()
    {
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
