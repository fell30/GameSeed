using System.Collections;
using UnityEngine;
using TMPro;

public class Pistol : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage;
    public float range = 100f;
    public float force = 10f;
    public float fireRate;
    private float nextTimeToFire = 0f;

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
    public Animator animator;

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
                if (CanShootWithFireRate())
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
    public bool CanShootWithFireRate()
    {
        return currentClipAmmo > 0 && !isReloading && Time.time >= nextTimeToFire;
    }

    void Shoot()
    {
        nextTimeToFire = Time.time + fireRate;
        currentClipAmmo--;
        UpdateAmmoUI();
        animator.SetTrigger("Shoot");


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
            float finalDamage = damage; // damage dasar


            // CEK APAKAH KENA KEPALA
            if (hit.collider.CompareTag("Head"))
            {
                finalDamage = damage * 3f; // HEADSHOT 3x damage

                Debug.Log("HEADSHOT! Damage: " + finalDamage);
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                finalDamage = damage * 0.7f; // LIMBS reduced damage

                Debug.Log("Limb shot! Damage: " + finalDamage);
            }
            else
            {
                // RANDOM CRITICAL (5% chance)
                if (Random.Range(0f, 100f) <= 5f)
                {
                    finalDamage = damage * 1.5f;

                    Debug.Log("CRITICAL HIT! Damage: " + finalDamage);
                }
                else
                {
                    Debug.Log("Body shot! Damage: " + finalDamage);
                }
            }

            // CARI ZOMBIE DARI HIT OBJECT ATAU PARENT-NYA
            ZombieEnemy zombieEnemy = hit.transform.GetComponent<ZombieEnemy>();
            ZombieFast zombieFast = hit.transform.GetComponent<ZombieFast>();

            // KALAU GA ADA DI HIT OBJECT, CARI DI PARENT
            if (zombieEnemy == null && zombieFast == null)
            {
                zombieEnemy = hit.transform.GetComponentInParent<ZombieEnemy>();
                zombieFast = hit.transform.GetComponentInParent<ZombieFast>();
            }

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * force);

            // APPLY DAMAGE DENGAN TYPE
            if (zombieEnemy != null)
                zombieEnemy.TakeDamage(finalDamage);

            if (zombieFast != null)
                zombieFast.TakeDamage(finalDamage);

            GameObject impactGO = Instantiate(
                hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Head") ? hitEnemyEffect : hitGroundEffect,
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
