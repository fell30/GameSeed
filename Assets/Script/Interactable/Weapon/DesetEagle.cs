using System.Collections;
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
    public GameObject hitEnemyEffect;
    public GameObject hitGroundEffect;

    public StressReceiver stressReceiver;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

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
                Shoot();
            else
                Debug.Log("No ammo! Press R to reload");
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            if (currentClipAmmo < maxClipSize && currentTotalAmmo > 0)
                StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentClipAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (stressReceiver != null)
            stressReceiver.InduceStress(0.15f);

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            TowerHealth towerHealth = hit.transform.GetComponent<TowerHealth>();
            ZombieEnemy zombieEnemy = hit.transform.GetComponent<ZombieEnemy>();
            ZombieFast zombieFast = hit.transform.GetComponent<ZombieFast>();

            if (towerHealth != null)
                towerHealth.TakeDamage(damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * force);

            if (zombieEnemy != null)
                zombieEnemy.TakeDamage(damage);

            if (zombieFast != null)
                zombieFast.TakeDamage(damage);

            GameObject impactGO = Instantiate(
                hit.transform.CompareTag("Enemy") ? hitEnemyEffect : hitGroundEffect,
                hit.point,
                Quaternion.LookRotation(hit.normal)
            );
            Destroy(impactGO, 2f);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
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
    }

    public int GetCurrentClipAmmo() => currentClipAmmo;
    public int GetCurrentTotalAmmo() => currentTotalAmmo;
    public bool IsReloading() => isReloading;
    public bool CanShoot() => currentClipAmmo > 0 && !isReloading;
}
