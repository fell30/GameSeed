using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    [Header("Pistol Specific")]
    public GameObject bulletHolePrefab;
    public float bulletForce = 30f;

    public override void Fire()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }

        // Set waktu untuk tembakan berikutnya
        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;

        // Play effects
        PlayShootEffects();

        // Gunakan direction yang akurat dari kamera
        Vector3 shootDirection = GetShootDirection();
        Vector3 rayOrigin = firePoint != null ? firePoint.position : transform.position;

        // Raycast untuk hit detection
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, shootDirection, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name + " at distance: " + hit.distance);


            Debug.DrawRay(rayOrigin, shootDirection * hit.distance, Color.red, 1f);




            if (bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHole, 10f);
            }

            // Add force jika object punya Rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-hit.normal * bulletForce);
            }
        }
        else
        {
            // Debug line untuk miss
            Debug.DrawRay(rayOrigin, shootDirection * range, Color.yellow, 1f);
        }

        Debug.Log(weaponName + " fired! Ammo left: " + currentAmmo);
    }
}