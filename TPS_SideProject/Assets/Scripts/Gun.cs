﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready,
        Empty,
        Reloading
    }

    public State state { get; private set; }

    public AudioClip shotClip;
    public AudioClip reloadClip;
    public ParticleSystem muzzleFlashEffect;
    public ParticleSystem shellEjectEffect;
    public Transform fireTransform;
    public Transform leftHandMount;

    public float damage = 25f;
    public float fireDistance = 100f;

    public int ammoRemain = 100;
    public int magAmmo;
    public int magCapacity = 30;

    public float timeBetFire = 0.12f;
    private float reloadTime = 1.2f;

    [Range(0f, 10f)] public float maxSpread = 3f;
    [Range(1f, 10f)] public float stability = 1f;
    [Range(0.01f, 10f)] public float restoreFromRecoilSpeed = 2f;

    private PlayerShooter gunHolder;
    private LineRenderer bulletLineRenderer;
    private AudioSource gunAudioPlayer;


    private float currentSpread;
    private float currentSpreadVelocity;

    private float lastFireTime;

    private LayerMask excludeTarget;

    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    public void SetUp(PlayerShooter gunHolder)
    {
        this.gunHolder = gunHolder;
        excludeTarget = gunHolder.excludeTarget;
    }
    private void OnEnable()
    {
        magAmmo = magCapacity;
        currentSpread = 0f;
        lastFireTime = 0f;
        state = State.Ready;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public bool Fire(Vector3 aimTarget)
    {
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            var fireDir = aimTarget - fireTransform.position;

            var xError = Utility.GetRandomNormalDistribution(0f, currentSpread);
            var yError = Utility.GetRandomNormalDistribution(0f, currentSpread);

            fireDir = Quaternion.AngleAxis(yError, Vector3.up) * fireDir;
            fireDir = Quaternion.AngleAxis(xError, Vector3.right) * fireDir;


            currentSpread += 1f / stability;

            lastFireTime = Time.time;
            Shot(fireTransform.position, fireDir);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void Shot(Vector3 startPoint, Vector3 direction)
    {
        RaycastHit hit;
        Vector3 hitPosition;

        if (Physics.Raycast(startPoint, direction, out hit, fireDistance, ~excludeTarget) == true)
        {
            var target = hit.collider.GetComponent<IDamageble>();

            if (target != null)
            {
                DamageMessage damageMessage;
                damageMessage.damager = gunHolder.gameObject;
                damageMessage.amount = damage;
                damageMessage.hitPoint = hit.point;
                damageMessage.hitNormal = hit.normal;

                target.ApplyDamage(damageMessage);
            }
            else
            {
                EffectManager.Instance.PlayHitEffect(hit.point, hit.normal, hit.transform);
            }
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = startPoint + direction * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));
        --magAmmo;
        if (magAmmo <= 0)
        {
            state = State.Empty;
        }
    }


    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();

        gunAudioPlayer.PlayOneShot(shotClip);

        bulletLineRenderer.enabled = true;
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);

        yield return new WaitForSeconds(0.03f);

        bulletLineRenderer.enabled = false;
    }

    public bool Reload()
    {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)
        {
            return false;
        }
        else
        {
            StartCoroutine(ReloadRoutine());
            return true;
        }
    }

    private IEnumerator ReloadRoutine()
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        var ammoToFill = Mathf.Clamp(magCapacity - magAmmo, 0, ammoRemain);
        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;

        state = State.Ready;
    }

    private void Update()
    {
        currentSpread = Mathf.Clamp(currentSpread, 0, maxSpread);
        currentSpread = Mathf.SmoothDamp(currentSpread, 0f, ref currentSpreadVelocity, 1f / restoreFromRecoilSpeed);
    }
}
