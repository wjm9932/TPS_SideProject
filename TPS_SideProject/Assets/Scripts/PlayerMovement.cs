 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float jumpVelocity = 20f;
    [Range(0.01f, 1f)] public float airControlPercent;
    public float speedSmoothTime = 0.1f;
    public float turnSmoothTime = 0.1f;

    private CharacterController characterController;
    private PlayerInput input;
    private PlayerShooter playerShooter;
    private Animator animator;
    private Camera followCamera;

    private float speedSmoothVelocity;
    private float turnSmoothVelocity;
    private float currnetVelocityY;
    public float currentSpeed => new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;


    // Start is called before the first frame update
    void Start()
    {
        playerShooter = GetComponent<PlayerShooter>();
        input = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        followCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (currentSpeed > 2.0f || input.isFire || playerShooter.aimState == PlayerShooter.AimState.HipFire)
        {
            Rotate();
        }
        
        Move(input.moveInput);
        
        if(input.isJump)
        {
            //Jump();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateAnimation(input.moveInput);
    }
    
    public  void Move(Vector2 moveInput)
    {
        float targetSpeed = speed * moveInput.magnitude;
        Vector3 moveDir = Vector3.Normalize(transform.forward * moveInput.y + transform.right * moveInput.x);

        var smoothTime = characterController.isGrounded ? speedSmoothTime : speedSmoothTime / airControlPercent;

        targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);

        currnetVelocityY += Time.deltaTime * Physics.gravity.y;

        Vector3 velocity = moveDir * targetSpeed + Vector3.up * currnetVelocityY;

        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded == true)
        {
            currnetVelocityY = 0f;
        }

    }
    
    public void Rotate()
    {
        var targetRotation = followCamera.transform.eulerAngles.y;

        targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

        transform.eulerAngles = Vector3.up * targetRotation;
    }

    
    public void Jump()
    {
        if(characterController.isGrounded == true)
        {
            currnetVelocityY = jumpVelocity;
        }
        else
        {
            return;
        }
    }

    private void UpdateAnimation(Vector2 moveInput)
    {
        var animationSpeedPercent =  currentSpeed / speed;
        animator.SetFloat("Vertical", moveInput.y, 0.05f, Time.deltaTime);
        animator.SetFloat("Horizontal", moveInput.x, 0.05f, Time.deltaTime);
    }
}
