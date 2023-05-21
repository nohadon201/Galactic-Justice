using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

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
    [SerializeField]
    private LayerMask obstructionMask;

    NetworkObject no;
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
    public void CalculateAccuracy()
    {
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