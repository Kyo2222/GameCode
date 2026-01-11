using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingController : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private float fireRate = 0.1f;
    [SerializeField]
    private float fireRange = 10f;
    private float nextFireTime;
    [SerializeField]
    private bool isAuto;
    [SerializeField]
    private int maxAmmo = 30;
    private int currentAmmo;
    [SerializeField]
    private float reloadTime = 1.5f;
    private bool isReloading;
    [SerializeField]
    private ParticleSystem muzzleFlash;
    [SerializeField]
    private ParticleSystem bloodEffect;
    [SerializeField]
    private int damagePerShot = 10;

    [Header("Sound")]
    private AudioSource soundAudioSource;
    [SerializeField]
    private AudioClip shootingSoundClip;
    [SerializeField]
    private AudioClip reloadSoundClip;
    [SerializeField]
    private Text currentAmmoText;

    private void Start()
    {
        animator = GetComponent<Animator>();
        var playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
            soundAudioSource = playerObject.GetComponent<AudioSource>();
        else
            Debug.Log("Player object not found!");
        
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        currentAmmoText.text = currentAmmo.ToString();

        if(isReloading)
            return;

        if(isAuto)
        {
            if(Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
                animator.SetBool("Shoot", false);
        }
        else
        {
            if(Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
                animator.SetBool("Shoot", false);
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
            Reload();
    }
    private void Shoot()
    {
        if(currentAmmo > 0)
        {
            RaycastHit hit;
            if(Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
            {
                Debug.Log(hit.transform.name);

                //damage to zombie 
                var zombie = hit.collider.GetComponent<IDamagable>();
                if(zombie != null)
                {
                    zombie.TakeDamage(damagePerShot);

                    //blood effect hit point.
                    ParticleSystem blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(blood.gameObject, blood.main.duration);
                }
            }

            muzzleFlash.Play();
            animator.SetBool("Shoot", true);
            currentAmmo--;

            soundAudioSource.PlayOneShot(shootingSoundClip);
        }
        else
        {
            //Reload
            Reload();
        }
    }

    private void Reload()
    {
        if (isReloading || currentAmmo >= maxAmmo) 
            return;
        
        animator.SetTrigger("Reload");
        if(GameManager.Instance.CurrentWeaponIndex == 2)
            animator.speed = 2f;
        
        isReloading = true;
        //play reload sound
        soundAudioSource.PlayOneShot(reloadSoundClip);
        Invoke("FinishReloading", reloadTime);
    }

    private void FinishReloading()
    {
        animator.speed = 1f;
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.ResetTrigger("Reload");
    }
}
