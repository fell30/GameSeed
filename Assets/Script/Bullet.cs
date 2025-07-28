using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage = 25f;
    [HideInInspector] public float range = 100f;
    [HideInInspector] public Vector3 origin;

    [Header("Bullet Settings")]
    public GameObject hitEffect;
    public float bulletDrag = 0f;
    public bool useGravity = false;
    public LayerMask hitLayers = -1;

    [Header("Anti-Tunneling")]
    public bool useRaycastDetection = true; // Solusi terbaik untuk high speed

    private Rigidbody rb;
    private bool hasHit = false;
    private Vector3 previousPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Set collision detection untuk high speed bullets
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.drag = bulletDrag;
            rb.useGravity = useGravity;
        }

        origin = transform.position;
        previousPosition = transform.position;
    }

    void Update()
    {
        // Cek jarak maksimum
        if (Vector3.Distance(origin, transform.position) > range)
        {
            DestroyBullet();
            return;
        }

        // Anti-tunneling dengan raycast detection
        if (useRaycastDetection && !hasHit)
        {
            CheckRaycastHit();
        }

        previousPosition = transform.position;
    }

    void CheckRaycastHit()
    {
        Vector3 direction = transform.position - previousPosition;
        float distance = direction.magnitude;

        if (distance > 0.1f) // Minimal distance untuk raycast
        {
            RaycastHit hit;
            if (Physics.Raycast(previousPosition, direction.normalized, out hit, distance, hitLayers))
            {
                if (!hasHit)
                {
                    // Move bullet ke posisi hit
                    transform.position = hit.point;
                    ProcessHit(hit.collider);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Cek layer
        if (((1 << other.gameObject.layer) & hitLayers) == 0) return;

        ProcessHit(other);
    }

    void ProcessHit(Collider hitCollider)
    {
        if (hasHit) return;
        hasHit = true;

        // Damage enemy
        EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"Bullet hit {hitCollider.name} for {damage} damage");
        }

        // Add force
        Rigidbody hitRb = hitCollider.GetComponent<Rigidbody>();
        if (hitRb != null && rb != null)
        {
            hitRb.AddForce(rb.velocity.normalized * 500f);
        }

        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.LookRotation(-transform.forward));
        }

        DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
