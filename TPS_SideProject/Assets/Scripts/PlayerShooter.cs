using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public enum AimState
    {
        Idle,
        HipFire
    }

    public AimState aimState { get; private set; }

    public LayerMask excludeTarget;
    public Gun gun;
    private PlayerInput playerInput;
    private Animator playerAnimator;
    private Camera playerCamera;

    private Vector3 aimPoint;
    private bool linedUp => !(Mathf.Abs(playerCamera.transform.eulerAngles.y - transform.eulerAngles.y) > 1f);
    private bool hasEnoughDistance => !Physics.Linecast(transform.position + Vector3.up * gun.fireTransform.position.y, gun.transform.position, ~excludeTarget);

    private void Awake()
    {
        if (excludeTarget != (excludeTarget | (1 << gameObject.layer)))
        {
            excludeTarget |= 1 << gameObject.layer;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
        gun.SetUp(this);
    }
    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (playerInput.isFire == true)
        {
            Shoot();
        }
    }
    // Update is called once per frame
    void Update()
    {
        var angle = playerCamera.transform.eulerAngles.x;
        if(angle > 270)
        {
            angle -= 360f;
        }
        angle = angle / -180f + 0.5f;
        playerAnimator.SetFloat("Angle", angle);
        
        UpdateAimTarget();
        
        if (playerInput.isReload == true)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (gun.Reload() == true)
        {
            playerAnimator.SetTrigger("Reload");
        }
    }

    public void Shoot()
    {
        if (aimState == AimState.Idle)
        {
            if (linedUp == true)
            {
                aimState = AimState.HipFire;
            }
        }
        else if (aimState == AimState.HipFire)
        {
            if (hasEnoughDistance == true)
            {
                if (gun.Fire(aimPoint) == true)
                {
                    playerAnimator.SetTrigger("Shoot");
                }
            }
            else
            {
                aimState = AimState.Idle;
            }
        }
    }

    private void UpdateAimTarget()
    {
        RaycastHit hit;
        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out hit, gun.fireDistance, ~excludeTarget) == true)
        {
            aimPoint = hit.point;

            if (Physics.Linecast(gun.fireTransform.position, hit.point, out hit, ~excludeTarget) == true) // intersect
            {
                aimPoint = hit.point;
            }
        }
        else
        {
            aimPoint = playerCamera.transform.position + playerCamera.transform.forward * gun.fireDistance;
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if(gun == null || gun.state == Gun.State.Reloading)
        {
            return;
        }
        else
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, gun.leftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, gun.leftHandMount.rotation);
        }
    }
}
