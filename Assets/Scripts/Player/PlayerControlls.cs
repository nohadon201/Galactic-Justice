using Cinemachine;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControlls : NetworkBehaviour
{
    /*
     * ################################### Events And Delegators ##############################################
     */
    public delegate void DisplayInterface();
    public DisplayInterface displayInterfaceDelegator;

    public delegate void DisplayPause();
    public DisplayPause displayPauseDelegator;

    public delegate void ChangeSlot();
    public DisplayInterface changeSlotDelegator;

    public delegate void GoToNextInterface(bool b);
    public GoToNextInterface goToNextInterfaceDelegator;

    [SerializeField] private GameEvent OnPlayerJump, OnPlayerMoveEvent;
    [SerializeField] private EventPoints WinPointsEvent;

    [Header("Configs")]
    /*
     * ################################### Camera ##############################################
     */
    [SerializeField] private float Sensibility;

    private GameObject CameraTarget;

    private Vector2 directionMovement, directionRotationOfCamera;

    private float cameraRotation;

    /*
     * ################################### Components Of GameObject ##############################################
     */

    private Rigidbody rb;

    private PlayerWeapon weapon;

    private PowerBullets powerBullets;

    /*
     * ################################### Dash ##############################################
     */
    private float dashForce, dashingTime, dashCooldown;


    private Coroutine RegenerationOfAmmunition, RegenerationShieldCoroutine;

    private bool Jump1, Jump2, CanDash, Dashing, RegenerationShield, Interface, Pause, Step;
    public bool pushed;

    [SerializeField] public PlayerInfo OwnInfo;

    public delegate void EndLevel();
    public EndLevel EndLevelDelegator;

    private GameObject VirtualCamera;
    public GameObject Camera;
    [SerializeField]
    private Transform StepDetectorTransformForward;
    [SerializeField]
    private Transform StepDetectorTransformRight;
    [SerializeField]
    private Transform StepDetectorTransformLeft;
    [SerializeField]
    private Transform StepDetectorTransformBehind;

    public void DefaultValues()
    {
        Step = false;
        pushed = false;
        Interface = false;
        Jump1 = false;
        Jump2 = false;
        CanDash = true;
        Dashing = false;
        RegenerationShield = false;

        dashForce = 24f;
        dashCooldown = 1f;
        dashingTime = 0.3f;

        rb = GetComponent<Rigidbody>();
        weapon = GetComponent<PlayerWeapon>();
        powerBullets = GetComponent<PowerBullets>();

        if ((!IsServer && IsOwner) || (IsServer && !IsOwner))
            OwnInfo = Resources.Load<PlayerInfo>("Player/Client/ClientPlayerInformation");
        else
            OwnInfo = Resources.Load<PlayerInfo>("Player/Host/HostPlayerInformation");

        OwnInfo.DefaultValues(IsServer);

        foreach (Skills sk in OwnInfo.abilities)
        {
            sk.initValues();
        }

        if (CameraTarget == null)
            CameraTarget = transform.GetChild(0).gameObject;


        if (!IsOwner) return;

        Camera oldCamera = FindObjectOfType<Camera>();
        oldCamera.gameObject.GetComponent<AudioListener>().enabled = false;

        Camera = Instantiate(Resources.Load<GameObject>("Player/MainCamera"));

        GameObject a = Resources.Load<GameObject>("Player/CameraCinemachine");

        a.GetComponent<CinemachineVirtualCamera>().Follow = CameraTarget.transform;

        VirtualCamera = Instantiate(a);

        if (oldCamera != null && oldCamera.enabled)
            oldCamera.enabled = false;

        weapon.camera = Camera.GetComponent<Camera>();

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (Dashing)
            rb.velocity = transform.forward * dashForce;
    }
    public bool Fall()
    {
        return transform.position.y < -15;
    }
    void LateUpdate()
    {
        if (!IsOwner) return;
        if (Fall())
        {
            if (SceneManager.GetActiveScene().name != "Lvl1")
                OnPlayerDeath();
            else
                transform.position = new Vector3(0, 0, 0);

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
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
        else if (Input.GetKeyDown(KeyCode.P))
        {
            EndLevelDelegator?.Invoke();
        }
        if (pushed) return;
        // Velocity of the player
        PlayerMovement();

        // Camera Rotation
        CameraRotation();
    }
    /**
     * ############################ NETWORK STATES ########################################### 
     */
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        DefaultValues();
    }
    /**
    * ############################ INTERFACE ########################################### 
    */
    public void OnInterfaceDisplayer(InputAction.CallbackContext context)
    {
        if (!IsOwner || Dashing || Pause) return;
        if (context.canceled)
        {
            displayInterfaceDelegator.Invoke();

            if (Interface)
                powerBullets.LoadPointsPowers();

            Interface = Interface ? false : true;
        }
    }
    public void OnPauseGame(InputAction.CallbackContext context)
    {
        if (!IsOwner || Dashing) return;
        if (context.canceled)
        {
            displayPauseDelegator?.Invoke();
            Pause = Pause ? false : true;
        }
    }
    /**
     * ############################ SHOOTING ########################################### 
     */
    public void OnClick(InputAction.CallbackContext context)
    {
        if (Dashing || !IsOwner || Interface || Pause) return;

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
            //      NOTA PARAR CORRUTINA DE DISPARAR
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
        if (directionMovement.x > 0)
        {
            rb.velocity = new Vector3(transform.right.x * OwnInfo.playerVelocity, rb.velocity.y, transform.right.z * OwnInfo.playerVelocity);
            //RaycastStep(StepDetectorTransformRight.position, StepDetectorTransformRight.right, 100f);
        }
        else if (directionMovement.x < 0)
        {
            rb.velocity = new Vector3(-transform.right.x * OwnInfo.playerVelocity, rb.velocity.y, -transform.right.z * OwnInfo.playerVelocity);
            //RaycastStep(StepDetectorTransformLeft.position, -StepDetectorTransformLeft.right, 100f);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        //      +Z
        if (directionMovement.y > 0)
        {
            rb.velocity += new Vector3(transform.forward.x, 0, transform.forward.z) * OwnInfo.playerVelocity;
            //RaycastStep(StepDetectorTransformForward.position, StepDetectorTransformForward.forward, 100f);
        }
        else if (directionMovement.y < 0)
        {
            rb.velocity += new Vector3(-transform.forward.x, 0, -transform.forward.z) * OwnInfo.playerVelocity;
            //RaycastStep(StepDetectorTransformBehind.position, -StepDetectorTransformBehind.forward, 100f);
        }
        //if (Step)
        //    StartCoroutine(StepForceCooldown(0.4f));
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
        if (CanDash && IsOwner && !Interface && !Pause)
            StartCoroutine(Dash());
    }
    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        if (Dashing || !IsOwner || Interface || Pause) return;
        try
        {
            Vector2 v2 = context.ReadValue<Vector2>();
            directionRotationOfCamera = v2;
        }
        catch (System.Exception e)
        {

        }
    }
    private void RaycastStep(Vector3 position, Vector3 direction, float force)
    {
        if (Physics.Raycast(position, direction, 0.5f, 7) && !Step)
        {
            Step = true;
            rb.AddForce(transform.up * force, ForceMode.Force);
        }
        Debug.DrawLine(position, position + (direction), Color.red, 0.5f);
    }
    private IEnumerator StepForceCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        Step = false;
    }
    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        if (Dashing || !IsOwner || Interface || Pause) return;
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
        if (Dashing || !IsOwner || Interface || Pause) return;

        if (context.performed)
        {
            if (!Jump1 && !Jump2)
            {
                Jump1 = true;
                rb.AddForce(transform.up * 200);
            }
            else if (!Jump2)
            {
                Jump2 = true;
                rb.AddForce(transform.up * 200);
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
        if (!IsOwner || Pause) return;
        if (!Interface)
        {
            changeSlotDelegator?.Invoke();
            if (context.performed && !weapon.Regeneration)
            {
                weapon.IndexCurrentConfiguration = weapon.IndexCurrentConfiguration + 1 == weapon.WeaponConfigurations.Count ? 0 : weapon.IndexCurrentConfiguration + 1;
                weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration].CurrentAmmunition = weapon.CurrentConfiguration.CurrentAmmunition;
                weapon.CurrentConfiguration = weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration];
                weapon.CurrentConfiguration.LoadConfigurationOfWeapon();
            }
            else if (context.performed && weapon.Regeneration)
            {
                StopCoroutine(RegenerationOfAmmunition);
                weapon.IndexCurrentConfiguration = weapon.IndexCurrentConfiguration + 1 == weapon.WeaponConfigurations.Count ? 0 : weapon.IndexCurrentConfiguration + 1;
                weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration].CurrentAmmunition = weapon.CurrentConfiguration.CurrentAmmunition;
                weapon.CurrentConfiguration = weapon.WeaponConfigurations[weapon.IndexCurrentConfiguration];
                weapon.CurrentConfiguration.LoadConfigurationOfWeapon();
                RegenerationOfAmmunition = StartCoroutine(RegenerateAmunition());
            }
        }
        else
        {
            goToNextInterfaceDelegator?.Invoke(true);
        }
    }
    public void OnLeftSlot(InputAction.CallbackContext context)
    {
        if (!IsOwner || Pause) return;
        if (!Interface)
        {
            changeSlotDelegator?.Invoke();
            if (context.performed && !weapon.Regeneration)
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
        else
        {
            goToNextInterfaceDelegator?.Invoke(false);
        }
    }


    /**
     * ############################ Health and shield ########################################### 
     */

    public void OnCollisionEnter(Collision collision)
    {
        if (pushed) pushed = false;
        TouchFloor();
    }

    public void Damage(float damage)
    {
        Debug.Log("AU! Damage:" + damage + ", HealthBeforeImpact: " + OwnInfo.playersCurrentHealth + ", Shield: " + OwnInfo.playersCurrentShield);
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
            if (OwnInfo.playersCurrentHealth <= 0)
            {
                OwnInfo.playersCurrentHealth = 0;
                Debug.Log("Muelto");
                //this.gameObject.SetActive(false);
            }
            RegenerationShieldCoroutine = StartCoroutine(RegenerationOfShield());
        }
        //GetDamageClientRpc(damage);
    }
    [ClientRpc]
    public void GetPushedClientRpc(Vector3 force, bool host)
    {
        if ((host && !IsServer) || (!host && IsServer)) return;
        pushed = true;
        rb.AddForce(force, ForceMode.Impulse);
    }
    [ClientRpc]
    public void GetDamageClientRpc(float damage, bool host)
    {
        if ((host && !IsServer) || (!host && IsServer)) return;
        Debug.Log("AU! Damage:" + damage + ", HealthBeforeImpact: " + OwnInfo.playersCurrentHealth + ", Shield: " + OwnInfo.playersCurrentShield);
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
            if (OwnInfo.playersCurrentHealth <= 0)
            {
                OwnInfo.playersCurrentHealth = 0;
                Debug.Log("Muelto");
                //this.gameObject.SetActive(false);
            }
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
    }
    private void OnPlayerDeath()
    {

    } 
    /**
     * ############################ Temporal Skills ########################################### 
     */
    public void ActivateSkill(int num)
    {
        if (num < OwnInfo.abilities.Count && num >= 0)
        {
            Debug.Log(weapon.CurrentConfiguration.DamageBaseWeapon);
            StartCoroutine(OwnInfo.abilities[num].SkillCoroutine(OwnInfo, gameObject));
            OwnInfo.abilities.Remove(OwnInfo.abilities[num]);
            Debug.Log(weapon.CurrentConfiguration.DamageBaseWeapon);
        }
    }
    /**
     * ############################ Points ########################################### 
     */
    public void WinPoints(int points)
    {
        if (IsOwner)
        {
            OwnInfo.TotalPoints += points;
            OwnInfo.Points += points;
            if (IsServer) WinPointsClientRpc(points);
        }
    }
    [ClientRpc]
    private void WinPointsClientRpc(int points)
    {
        if (IsServer) return;
        WinPointsEvent?.Raise(points);
    }
    /**
     * ############################ PauseActions ########################################### 
     */

    //      CLIENT
    public void DisconnectClient()
    {
        NetworkManager.Singleton.DisconnectClient(GetComponent<NetworkObject>().OwnerClientId);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    //      HOST
    public void GoToLevelMenu()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("LevelMenu", LoadSceneMode.Single);
    }
    public void BackToMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }
    public void SaveGame()
    {
        SaveGameManager.Singleton.SaveClientRpc();
    }
}

