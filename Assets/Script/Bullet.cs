using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] GameObject ImpactEffect;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {

            ZombieEnemy enemy = col.GetComponentInParent<ZombieEnemy>();
            ZombieFast enemyFast = col.GetComponentInParent<ZombieFast>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (enemyFast != null)
            {
                enemyFast.TakeDamage(damage);
            }
            if (ImpactEffect != null)
            {
                GameObject impact = Instantiate(ImpactEffect, transform.position, Quaternion.identity);
                Destroy(impact, 2f);
            }

            Destroy(gameObject);
        }
    }
}