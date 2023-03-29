using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlls : MonoBehaviour
{
    //      Controls Variables
    [Header("Configs")]
    
    [SerializeField]
    private float Sensibility;

    private GameObject CameraTarget;

    private Vector2 directionMovement, directionRotationOfCamera;

    private Rigidbody rb;

    private float cameraRotation;

    private float dashForce,dashingTime,dashCooldown;

    private PlayerWeapon weapon;

    private Coroutine RegenerationOfAmmunition,RegenerationShieldCoroutine;

    private bool Jump1, Jump2, CanDash, Dashing, RegenerationShield;
    [SerializeField]
    private PlayerInfo OwnInfo;


    void Awake()
    {
        DefaultValues();
    }
    public void DefaultValues()
    {
        Jump1 = false;
        Jump2 = false;
        CanDash = true;
        Dashing = false;
        RegenerationShield = false;
        dashForce = 24f;
        dashCooldown = 1f;
        dashingTime = 0.3f;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<PlayerWeapon>();
        OwnInfo.DefaultValues();
        foreach(Skills sk in OwnInfo.abilities)
        {
            sk.initValues();
        }
        if (CameraTarget == null)
        {
            CameraTarget = transform.GetChild(0).gameObject;
        }
    }
    private void FixedUpdate()
    {
        if(Dashing)
        {
            rb.velocity = transform.forward * dashForce;
        }
    }
    void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            ActivateSkill(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateSkill(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateSkill(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateSkill(3);
        }
        // Velocity of the player
        PlayerMovement();

        // Camera Rotation
        CameraRotation();
    }
    /**
     * ############################ SHOOTING ########################################### 
     */

    public void OnClick(InputAction.CallbackContext context)
    {
        if (Dashing) return;
        if (context.started)
        {
            if (weapon.CanShoot())
            {
                if (weapon.Regeneration)
                {
                    StopCoroutine(RegenerationOfAmmunition);
                    weapon.Regeneration = false;    
                }
                weapon.Shooting = true;
                StartCoroutine(ShootFrequency());
            }
            else
            {
                Debug.Log(weapon.CurrentConfiguration.CurrentAmmunition);
            }
        }
        else if (context.canceled && !weapon.Regeneration)
        {
            weapon.Shooting = false;
            RegenerationOfAmmunition = StartCoroutine(RegenerateAmunition());
        }
    }
    private IEnumerator ShootFrequency()
    {
        while (weapon.Shooting)
        {
            weapon.Shoot();
            yield return new WaitForSeconds(weapon.CurrentConfiguration.CurrentCooldownBetweenBullets);
        }
    }
    public IEnumerator RegenerateAmunition()
    {
        weapon.Regeneration = true;
        float regeneration_value = weapon.CurrentConfiguration.RegenerationValueAmmunition;
        yield return new WaitForSeconds(1.5f);
        while (!weapon.AmmunitionEmpty())
        {
            weapon.RegenerateAmmunition(regeneration_value);
            yield return new WaitForSeconds(weapon.CurrentConfiguration.CurrentCooldownBetweenBullets * Time.deltaTime);
        }
        weapon.Regeneration = false;
    }

    /**
     * ############################ MOVEMENT AND CAMERA CONTROLS ########################################### 
     */
    public void CameraRotation()
    {
        
        transform.Rotate(Vector3.up * directionRotationOfCamera.x * Sensibility);

        cameraRotation += (-directionRotationOfCamera.y * Sensibility);

        cameraRotation = Mathf.Clamp(cameraRotation, -90, 90);

        CameraTarget.transform.eulerAngles = new Vector3(cameraRotation, CameraTarget.transform.eulerAngles.y, CameraTarget.transform.eulerAngles.z);
    }

    public void PlayerMovement()
    {
        //      X 
        Vector3 velocity;
        if (directionMovement.x > 0)
        {
            rb.velocity = new Vector3(transform.right.x * OwnInfo.playerVelocity, rb.velocity.y, transform.right.z * OwnInfo.playerVelocity);
        }
        else if (directionMovement.x < 0)
        {
            rb.velocity = new Vector3(-transform.right.x * OwnInfo.playerVelocity, rb.velocity.y, -transform.right.z * OwnInfo.playerVelocity);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        //      +Z

        if (directionMovement.y > 0)
        {
            rb.velocity += new Vector3(transform.forward.x, 0, transform.forward.z) * OwnInfo.playerVelocity;
        }
        else if (directionMovement.y < 0)
        {
            rb.velocity += new Vector3(-transform.forward.x, 0, -transform.forward.z) * OwnInfo.playerVelocity;
        }


    }
    public IEnumerator Dash()
    {
        CanDash = false;
        rb.useGravity = false;
        Dashing = true;
        yield return new WaitForSeconds(dashingTime);
        Dashing = false;
        rb.useGravity = true;
        yield return new WaitForSeconds(dashCooldown);
        CanDash = true;
    }
    public void OnDashing(InputAction.CallbackContext context)
    {
        if(CanDash)
        {
            StartCoroutine(Dash());
        }
    }
    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        if (Dashing) return;
        try
        {
            Vector2 v2 = context.ReadValue<Vector2>();
            directionRotationOfCamera = v2;
        }
        catch (System.Exception e)
        {

        }
    }
    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        if (Dashing) return;
        try
        {
            Vector2 v2 = context.ReadValue<Vector2>();
            directionMovement = v2;
        }
        catch (System.Exception e)
        {

        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (Dashing) return;
        if (context.performed)
        {
            if(!Jump1 && !Jump2)
            {
                Jump1 = true;
                rb.AddForce(transform.up * 300f);
            }
            else if (!Jump2)
            {
                Jump2 = true;
                rb.AddForce(transform.up * 300f);
            }
            
        }

    }
    public void TouchFloor()
    {
        Jump1 = false;   
        Jump2 = false;
    }

    /**
     * ############################ Slots of Memory ########################################### 
     */

    public void OnRightSlot(InputAction.CallbackContext context)
    {

        if(context.performed && !weapon.Regeneration)
        {
            weapon.IndexCurrentConfiguration = weapon.IndexCurrentConfiguration + 1 == weapon.WeaponConfigurations.Count ? 0 : weapon.IndexCurrentConfiguration + 1;
            weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration].CurrentAmmunition = weapon.CurrentConfiguration.CurrentAmmunition;
            weapon.CurrentConfiguration = weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration];
            weapon.CurrentConfiguration.LoadConfigurationOfWeapon();
        }else if(context.performed && weapon.Regeneration)
        {
            StopCoroutine(RegenerationOfAmmunition);
            weapon.IndexCurrentConfiguration = weapon.IndexCurrentConfiguration + 1 == weapon.WeaponConfigurations.Count ? 0 : weapon.IndexCurrentConfiguration + 1;
            weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration].CurrentAmmunition = weapon.CurrentConfiguration.CurrentAmmunition;
            weapon.CurrentConfiguration = weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration];
            weapon.CurrentConfiguration.LoadConfigurationOfWeapon();
            RegenerationOfAmmunition = StartCoroutine(RegenerateAmunition());
        }

    }
    public void OnLeftSlot(InputAction.CallbackContext context)
    {
        if(context.performed && !weapon.Regeneration)
        {
            weapon.IndexCurrentConfiguration = weapon.IndexCurrentConfiguration - 1 < 0 ? weapon.WeaponConfigurations.Count - 1 : weapon.IndexCurrentConfiguration - 1;
            weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration].CurrentAmmunition = weapon.CurrentConfiguration.CurrentAmmunition;
            weapon.CurrentConfiguration = weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration];
            weapon.CurrentConfiguration.LoadConfigurationOfWeapon();
        }
        else if (context.performed && weapon.Regeneration)
        {
            StopCoroutine(RegenerationOfAmmunition);
            weapon.IndexCurrentConfiguration = weapon.IndexCurrentConfiguration - 1 < 0 ? weapon.WeaponConfigurations.Count - 1 : weapon.IndexCurrentConfiguration - 1;
            weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration].CurrentAmmunition = weapon.CurrentConfiguration.CurrentAmmunition;
            weapon.CurrentConfiguration = weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration];
            weapon.CurrentConfiguration.LoadConfigurationOfWeapon();
            RegenerationOfAmmunition = StartCoroutine(RegenerateAmunition());
        }

    }


    /**
     * ############################ Health and shield ########################################### 
     */

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Floor")
        {
            TouchFloor();
        }
    }

    public void Damage(float damage)
    {
        if (OwnInfo.playersCurrentShield > 0)
        {
            if (RegenerationShield)
            {
                StopCoroutine(RegenerationShieldCoroutine);
            }
            OwnInfo.playersCurrentShield -= damage; 
            RegenerationShieldCoroutine = StartCoroutine(RegenerationOfShield());
        }
        else
        {
            if (RegenerationShield)
            {
                StopCoroutine(RegenerationShieldCoroutine);
            }
            OwnInfo.playersCurrentHealth -= damage;
            RegenerationShieldCoroutine = StartCoroutine(RegenerationOfShield());
        }
    }

    public IEnumerator RegenerationOfShield()
    {
        RegenerationShield = true;
        yield return new WaitForSeconds(1.5f);
        while (ShieldNotFull())
        {
            RegenerationShieldSum();
            yield return new WaitForSeconds(0.1f);
        }
        RegenerationShield = false;
    }
    public bool ShieldNotFull()
    {
        if (OwnInfo.playersCurrentShield > OwnInfo.playersMaxShield)
        {
            OwnInfo.playersCurrentShield = OwnInfo.playersMaxShield;
        }
        return OwnInfo.playersCurrentShield != OwnInfo.playersMaxShield;
    }
    public void RegenerationShieldSum()
    {
        OwnInfo.playersCurrentShield += OwnInfo.RegenerationShieldValue;
        Debug.Log("ShieldRegenerating: "+OwnInfo.playersCurrentShield);
    }

    /**
     * ############################ Temporal Skills ########################################### 
     */
    public void ActivateSkill(int num)
    {
        if(num<OwnInfo.abilities.Count && num>=0) {
            Debug.Log(weapon.CurrentConfiguration.DamageBaseWeapon);
            StartCoroutine(OwnInfo.abilities[num].SkillCoroutine(OwnInfo, gameObject));
            OwnInfo.abilities.Remove(OwnInfo.abilities[num]);
            Debug.Log(weapon.CurrentConfiguration.DamageBaseWeapon);
        }
    }
}
