using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Pistol : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage;
    public float range = 100f;
    public float force = 10f;

    [Header("Ammo Settings")]
    public int maxClipSize = 12;
    public int maxAmmo = 120;
    public float reloadTime = 2f;
    private int currentClipAmmo;
    private int currentTotalAmmo;
    private bool isReloading = false;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public StressReceiver stressReceiver;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    void Start()
    {
        currentClipAmmo = maxClipSize;
        currentTotalAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isReloading)
        {
            if (currentClipAmmo > 0)
            {
                Shoot();
            }
            else
            {
                Debug.Log("No ammo! Press R to reload");
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            if (currentClipAmmo < maxClipSize && currentTotalAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    void Shoot()
    {
        currentClipAmmo--;
        UpdateAmmoUI();

        muzzleFlash.Play();
        stressReceiver.InduceStress(0.15f);
        AudioSource.PlayClipAtPoint(shootSound, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            TowerHealth towerHealth = hit.transform.GetComponent<TowerHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * force);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(reloadSound, transform.position);
        }

        if (ammoText != null)
        {
            ammoText.text = "RELOADING...";
        }

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxClipSize - currentClipAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentTotalAmmo);

        currentClipAmmo += ammoToReload;
        currentTotalAmmo -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI();

        Debug.Log("Reload complete!");
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentClipAmmo}/{currentTotalAmmo}";
        }
    }

    public void AddAmmo(int amount)
    {
        currentTotalAmmo += amount;
        currentTotalAmmo = Mathf.Min(currentTotalAmmo, maxAmmo);
        UpdateAmmoUI();
        Debug.Log($"Added {amount} ammo. Total: {currentTotalAmmo}");
    }

    public int GetCurrentClipAmmo()
    {
        return currentClipAmmo;
    }

    public int GetCurrentTotalAmmo()
    {
        return currentTotalAmmo;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public bool CanShoot()
    {
        return currentClipAmmo > 0 && !isReloading;
    }
}