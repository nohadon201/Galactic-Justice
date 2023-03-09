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
    private float playerVelocity;
    
    [SerializeField]
    private float Sensibility;

    [SerializeField]
    private GameObject CameraTarget;
    
    private Vector2 directionMovement, directionRotationOfCamera;
    
    private Rigidbody rb;
    
    private float cameraRotation;
    
    private PlayerWeapon weapon;

    private Coroutine RegenerationOfAmmunition;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
       
        rb = GetComponent<Rigidbody>();
        
        weapon = GetComponent<PlayerWeapon>();   

        if(playerVelocity == 0)
        {
            playerVelocity = 3f;
        }

        if(CameraTarget == null)
        {
            CameraTarget = transform.GetChild(0).gameObject;
        }
        
    }

    private void Update()
    {
        
    }
    void LateUpdate()
    {
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
        if(context.started)
        {
            if (weapon.CanShoot())
            {
                if(RegenerationOfAmmunition!= null)
                {
                    StopCoroutine(RegenerationOfAmmunition);
                }
                weapon.Shooting = true;
                //weapon.Shoot();
                StartCoroutine(ShootFrequency());
            }
            else
            {
                Debug.Log(weapon.CurrentConfiguration.CurrentAmmunition);
            }
        }else if(context.canceled)
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
        yield return new WaitForSeconds(1.5f);
        Debug.Log("StartRegenerating");
        while (!weapon.AmmunitionEmpty())
        {
            weapon.RegenerateAmmunition();
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("EndRegeneration");
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
        if(directionMovement.x > 0)
        {
            rb.velocity = new Vector3(transform.right.x * playerVelocity, rb.velocity.y, transform.right.z * playerVelocity);   
        }
        else if (directionMovement.x < 0)
        {
            rb.velocity = new Vector3(-transform.right.x * playerVelocity, rb.velocity.y, -transform.right.z * playerVelocity);
        }
        else {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        //      +Z

        if (directionMovement.y > 0)
        {
            rb.velocity += new Vector3(transform.forward.x,0, transform.forward.z) * playerVelocity;
        }
        else if (directionMovement.y < 0)
        {
            rb.velocity += new Vector3(-transform.forward.x,0, -transform.forward.z) * playerVelocity;
        }
        

    }

    public void OnCameraRotate(InputAction.CallbackContext context)
    {
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
        try
        {
            Vector2 v2 = context.ReadValue<Vector2>();  
            directionMovement = v2; 
        }catch(System.Exception e) {
            
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("ª");
            rb.AddForce(transform.up * 300f);
        }

    }
}
