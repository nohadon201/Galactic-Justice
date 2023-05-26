using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWeapon : NetworkBehaviour
{
    //######################## Events ########################
    [SerializeField] private GameEvent SetTextFirst;
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
    [SerializeField]
    private LayerMask obstructionMask;

    NetworkObject no;

    //######################## Ray ########################
    [SerializeField] private GameObject rayObject;
    private GameObject instanceRay1, instanceRay2, instanceRay3, instanceRay4, instanceRay5, instanceRay6, instanceRay7, instanceRay8;
    private Coroutine displayeRayCoroutine;
    [SerializeField] private Transform firePoint;
    private Vector3[] direcitons = new Vector3[8];
    //      Functions that executes only at Start
    private void Start()
    {
        obstructionMask |= (1 << LayerMask.NameToLayer("Enemy"));
        obstructionMask |= (1 << LayerMask.NameToLayer("Obstruction"));
        
        no = GetComponent<NetworkObject>(); 
        powerBullets = GetComponent<PowerBullets>();
        
        if (IsServer && IsOwner)
            WeaponConfigurations = Resources.LoadAll<SlotOfMemory>("Player/Host/SlotOfMemory").ToList();
        else if (IsOwner) {
            WeaponConfigurations = Resources.LoadAll<SlotOfMemory>("Player/Client/SlotOfMemory").ToList();
        }
        
        IndexCurrentConfiguration = 0;
        CurrentConfiguration = WeaponConfigurations[IndexCurrentConfiguration];
        
        for (int i = 0; i < WeaponConfigurations.Count; i++)
        {

            WeaponConfigurations[i].defaultValues();

            WeaponConfigurations[i].DispersionValues();

            WeaponConfigurations[i].LoadConfigurationOfWeapon();

        }

        Shooting = false;
        
        if (!IsOwner) return;
        GameObject Interface = Instantiate(Resources.Load<GameObject>("Prefabs/Player/UI"));
        UIPlayerControlls InterfaceComponent = Interface.GetComponent<UIPlayerControlls>();
        Interface.GetComponent<Canvas>().worldCamera = GetComponent<PlayerControlls>().Camera.GetComponent<Camera>();
        InterfaceComponent.setValues(this.gameObject);
        
        SetTextFirst?.Raise();
        
        firePoint = GetComponent<PlayerControlls>().Camera.gameObject.transform.GetChild(0).GetChild(2);
        instanceRay1 = Instantiate(rayObject);
        instanceRay2 = Instantiate(rayObject);
        instanceRay3 = Instantiate(rayObject);
        instanceRay4 = Instantiate(rayObject);
        instanceRay5 = Instantiate(rayObject);
        instanceRay6 = Instantiate(rayObject);
        instanceRay7 = Instantiate(rayObject);
        instanceRay8 = Instantiate(rayObject);
        
        instanceRay1.SetActive(false);
        instanceRay2.SetActive(false);
        instanceRay3.SetActive(false);
        instanceRay4.SetActive(false);
        instanceRay5.SetActive(false);
        instanceRay6.SetActive(false);
        instanceRay7.SetActive(false);
        instanceRay8.SetActive(false);
        StartCoroutine(RotateFirePoint());
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
        //Debug.Log("Regenerating: " + CurrentConfiguration.CurrentAmmunition);
        if(CurrentConfiguration.CurrentAmmunition< 0) {  CurrentConfiguration.CurrentAmmunition = 0;}
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
    private void Update()
    {
        if (!IsOwner) return;
        if (instanceRay1.gameObject.activeSelf)
        {
            instanceRay1.transform.position = firePoint.position;
            instanceRay2.transform.position = firePoint.position;
            instanceRay3.transform.position = firePoint.position;
            instanceRay4.transform.position = firePoint.position;
            instanceRay5.transform.position = firePoint.position;
            instanceRay6.transform.position = firePoint.position;
            instanceRay7.transform.position = firePoint.position;
            instanceRay8.transform.position = firePoint.position;

            instanceRay1.transform.forward = direcitons[0];
            instanceRay2.transform.forward = direcitons[1];
            instanceRay3.transform.forward = direcitons[2];
            instanceRay4.transform.forward = direcitons[3];
            instanceRay5.transform.forward = direcitons[4];
            instanceRay6.transform.forward = direcitons[5];
            instanceRay7.transform.forward = direcitons[6];
            instanceRay8.transform.forward = direcitons[7];
        }
    }
    public void CalculateAccuracy()
    {
        if (displayeRayCoroutine == null)
        {
            displayeRayCoroutine = StartCoroutine(displayerRay(false));
        }
        else
        {
            StopCoroutine(displayeRayCoroutine);
            instanceRay1.SetActive(false);
            instanceRay2.SetActive(false);
            instanceRay3.SetActive(false);
            instanceRay4.SetActive(false);
            instanceRay5.SetActive(false);
            instanceRay6.SetActive(false);
            instanceRay7.SetActive(false);
            instanceRay8.SetActive(false);
            displayeRayCoroutine = StartCoroutine(displayerRay(true));
        }

        if (CurrentConfiguration.Accuracy == 1)
        {
            RayCastToServerRpc(transform.position, camera.transform.forward, false, CurrentConfiguration.MaxRange * CurrentConfiguration.Power, CurrentConfiguration.CurrentDamageWeapon, CurrentConfiguration.CurrentForce, no.OwnerClientId);
        }
        else
        {
            for (int a = 0; a < CurrentConfiguration.CurrentDispersion.Length; a++)
            {
                Vector3 shootDirection = camera.transform.forward
                                        + camera.transform.right * CurrentConfiguration.CurrentDispersion[a].x
                                        + camera.transform.up * CurrentConfiguration.CurrentDispersion[a].y;
                RayCastToServerRpc(transform.position, shootDirection.normalized, false, CurrentConfiguration.MaxRange * CurrentConfiguration.Power, CurrentConfiguration.CurrentDamageWeapon, CurrentConfiguration.CurrentForce, no.OwnerClientId);
               
            }
        }
    }
    private IEnumerator RotateFirePoint()
    {
        RaycastHit hit;
        while (true)
        {
            if(CurrentConfiguration.Accuracy == 1)
            {
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, CurrentConfiguration.MaxRange * CurrentConfiguration.Power, obstructionMask))
                {
                    direcitons[0] = (hit.point - firePoint.position).normalized;
                }
                else
                {
                    Vector3 point = camera.transform.position + (camera.transform.forward * (CurrentConfiguration.MaxRange * CurrentConfiguration.Power));
                    direcitons[0] = (point - firePoint.position).normalized;
                }
            }
            else
            {
                for (int a = 0; a < CurrentConfiguration.CurrentDispersion.Length; a++)
                {
                    Vector3 shootDirection = camera.transform.forward
                                            + camera.transform.right * CurrentConfiguration.CurrentDispersion[a].x
                                            + camera.transform.up * CurrentConfiguration.CurrentDispersion[a].y;

                    if (Physics.Raycast(camera.transform.position, shootDirection, out hit, CurrentConfiguration.MaxRange * CurrentConfiguration.Power, obstructionMask))
                    {
                        direcitons[a] = (hit.point - firePoint.position).normalized;
                    }
                    else
                    {
                        Vector3 point = camera.transform.position + (camera.transform.forward * (CurrentConfiguration.MaxRange * CurrentConfiguration.Power));
                        direcitons[a] = (point - firePoint.position).normalized;
                    }
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator displayerRay(bool anterior)
    {
        if (anterior) yield return new WaitForSeconds(0.2f);
        instanceRay1.SetActive(true);
        if(CurrentConfiguration.Accuracy != 1)
        {
            instanceRay2.SetActive(true);
            instanceRay3.SetActive(true);
            instanceRay4.SetActive(true);
            instanceRay5.SetActive(true);
            instanceRay6.SetActive(true);
            instanceRay7.SetActive(true);
            instanceRay8.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        instanceRay1.SetActive(false);
        if (CurrentConfiguration.Accuracy != 1)
        {
            instanceRay2.SetActive(false);
            instanceRay3.SetActive(false);
            instanceRay4.SetActive(false);
            instanceRay5.SetActive(false);
            instanceRay6.SetActive(false);
            instanceRay7.SetActive(false);
            instanceRay8.SetActive(false);
        }
    }
    [ServerRpc]
    public void RayCastToServerRpc(Vector3 origin, Vector3 v, bool byPowerBullet, float currentRange, float currentDamage, float currentForce, ulong clientId)
    {
        if (!byPowerBullet)
        {
            RestAmmunitionClientRpc(clientId);
        }

        RaycastHit hit;

        if (Physics.Raycast(origin, v, out hit,currentRange, obstructionMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 3f);
            if (hit.transform.tag.Contains("Physics"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * currentForce, hit.point);
            }
            else if (hit.transform.tag.Contains("Enemy"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * currentForce * 10, hit.point);
                EnemyBehaviour eb = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
                if (eb != null)
                {
                    eb.playerRef = transform;
                    eb.ChangeState(StateOfEnemy.FOLLOWING);
                    eb.GetHit(currentDamage);
                }
            }else if(hit.transform.tag == "Tutorialer")
            {
                hit.transform.parent.gameObject.GetComponent<Tutorialer>()?.OnDisparar();
            }
            if (!byPowerBullet) powerBullets.execute(hit, byPowerBullet, currentRange, currentDamage, currentForce);   
        }
    }
    [ClientRpc]
    private void RestAmmunitionClientRpc(ulong clientId)
    {
        if (!IsOwner || no.OwnerClientId != clientId) return;
        CurrentConfiguration.CurrentAmmunition -= CurrentConfiguration.CurrentWasteOfAmmunitionPerBullet;
    }
}