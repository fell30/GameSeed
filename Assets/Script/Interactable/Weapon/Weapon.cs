using UnityEngine;

// Base class untuk semua senjata
[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName;
    public float damage = 25f; // Ubah ke float untuk kompatibilitas
    public float fireRate = 0.5f; // Waktu delay antar tembakan
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    public float range = 100f;
    public float bulletSpeed = 1000f; // Kecepatan bullet (untuk projectile mode)

    [Header("Bullet Type")]
    public bool useProjectile = false; // Toggle antara hitscan vs projectile
    public GameObject bulletPrefab; // Prefab bullet untuk projectile mode

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public Transform firePoint; // Titik keluar peluru

    protected AudioSource audioSource;
    protected float nextTimeToFire = 0f;
    protected bool isReloading = false;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        // Input untuk menembak (bisa diubah sesuai kebutuhan)
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && !isReloading)
        {
            Fire();
        }

        // Input untuk reload
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    public abstract void Fire(); // Method abstract yang harus diimplementasi tiap senjata

    protected virtual void PlayShootEffects()
    {
        // Muzzle flash
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Sound effect
        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);
    }

    protected virtual System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading " + weaponName + "...");

        if (reloadSound != null && audioSource != null)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete!");
    }

    // Method untuk raycast hit detection yang akurat
    protected virtual bool PerformRaycast(out RaycastHit hit, Vector3 customDirection = default)
    {
        Vector3 rayOrigin;
        Vector3 rayDirection;

        // Jika ada custom direction (untuk spread), gunakan itu
        if (customDirection != default)
        {
            rayOrigin = firePoint != null ? firePoint.position : transform.position;
            rayDirection = customDirection;
        }
        else
        {
            // Gunakan center screen untuk aiming yang akurat
            Camera playerCamera = Camera.main;
            if (playerCamera == null)
                playerCamera = FindObjectOfType<Camera>();

            if (playerCamera != null)
            {
                // Raycast dari center screen
                Ray cameraRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                rayOrigin = cameraRay.origin;
                rayDirection = cameraRay.direction;

                // Jika ada firePoint, adjust origin tapi tetap gunakan camera direction
                if (firePoint != null)
                {
                    rayOrigin = firePoint.position;
                    // Bisa juga pakai direction dari camera ke crosshair point
                }
            }
            else
            {
                // Fallback jika tidak ada camera
                rayOrigin = firePoint != null ? firePoint.position : transform.position;
                rayDirection = firePoint != null ? firePoint.forward : transform.forward;
            }
        }

        return Physics.Raycast(rayOrigin, rayDirection, out hit, range);
    }

    // Method untuk spawn projectile bullet
    protected virtual void FireProjectile()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not assigned!");

            return;
        }


        Vector3 shootDirection = GetShootDirection();
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));

        // Set velocity
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = shootDirection * bulletSpeed;
        }

        // Set bullet properties
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
            bulletScript.range = range;
            bulletScript.origin = spawnPosition;
        }

        // Auto destroy bullet setelah waktu tertentu
        Destroy(bullet, range / bulletSpeed + 2f);
    }
    protected virtual Vector3 GetShootDirection()
    {
        Camera playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();

        if (playerCamera != null)
        {
            // Gunakan raycast dari center screen untuk akurasi maksimal
            Ray cameraRay = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            // Cek apakah ada obstacle di depan
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit, range))
            {
                // Arahkan ke titik yang di-hit kamera
                Vector3 firePosition = firePoint != null ? firePoint.position : transform.position;
                return (hit.point - firePosition).normalized;
            }
            else
            {
                // Jika tidak ada hit, gunakan direction kamera
                return cameraRay.direction;
            }
        }

        // Fallback
        return firePoint != null ? firePoint.forward : transform.forward;
    }
}