using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera cam;
    public GameObject bulletPrefarb;
    public float bulletSpeed;
    public Transform LHhandPoint;
    public Transform RHhandPoint;

    bool lHhand;

    private Vector3 destination;



    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            destination = hit.point;
        }
        else
        {
            destination = ray.GetPoint(1000f);
        }
        if (lHhand)
        {
            lHhand = false;
            InstantiateProjectile(LHhandPoint);
        }
        else
        {
            lHhand = true;
            InstantiateProjectile(RHhandPoint);
        }
    }

    private void InstantiateProjectile(Transform firePoint)
    {
        var ProjectileObj = Instantiate(bulletPrefarb, firePoint.position, quaternion.identity);
        ProjectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * bulletSpeed;
    }
}