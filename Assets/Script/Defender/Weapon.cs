using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Space(5)]
    [Header("General Settings")]
    public GameObject projectile;
    public Transform shootPoint;
    public float force = 100f;
    public float shootingDelay = 1f;

    [Space(5)]
    [Header("Effects")]
    public ParticleSystem muzzleFlash;

    [Space(5)]
    [Header("Additional Options")]
    public GameObject secondProjectile;
    public Transform secondShootPoint;

    [Space(5)]
    [Header("Sound Settings")]
    public AudioClip fireClip;
    AudioSource audioSource;

    [Header("Animation")]
    public Animator weaponAnimator;


    [HideInInspector] public bool canShoot = false;

    IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (muzzleFlash == null)
        {
            muzzleFlash = GetComponentInChildren<ParticleSystem>();
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
        }

        while (true)
        {
            yield return new WaitForSeconds(shootingDelay);

            if (canShoot)
            {
                GameObject bullet = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                bullet.GetComponent<Rigidbody>().AddForce(shootPoint.forward * force);
                if (weaponAnimator != null)
                {
                    weaponAnimator.SetTrigger("Shoot");

                }


                if (secondProjectile)
                {
                    GameObject bullet2 = Instantiate(secondProjectile, secondShootPoint.position, secondShootPoint.rotation);
                    bullet2.GetComponent<Rigidbody>().AddForce(secondShootPoint.forward * force);
                }

                if (audioSource && fireClip)
                    audioSource.PlayOneShot(fireClip);
                Debug.Log("Weapon fired");

                if (muzzleFlash != null)
                {
                    muzzleFlash.Play();
                }
            }
        }
    }
}