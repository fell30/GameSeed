using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender_AI : MonoBehaviour
{
    public Transform gunHead;
    public float dampingSpeed = 10f;
    public string targetTag = "Enemy";
    public float shootingDistance = 30f;

    [Header("Seek Animation")]
    public bool playAnimationClip;
    public float seekSpeed = 50f;
    public float rotateAngle = 70f;

    [Header("Control")]
    public bool isDefenderActive = true;

    // Internal variables
    Vector3 originalRotation;
    bool isActive;
    Transform target;
    Coroutine detectionCoroutine;

    void Start()
    {
        originalRotation = gunHead.localRotation.eulerAngles;

        if (isDefenderActive)
        {
            StartDetection();
        }
    }

    void Update()
    {
        if (!isDefenderActive) return;

        if (isActive)
        {
            if (target)
            {
                Vector3 lookPos = target.position - gunHead.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                gunHead.rotation = Quaternion.Slerp(gunHead.rotation, rotation, Time.deltaTime * dampingSpeed);
            }
        }
        else
        {
            gunHead.localRotation = Quaternion.Euler(originalRotation.x, Mathf.PingPong(Time.time * seekSpeed, rotateAngle * 2) - rotateAngle, 1f);
        }
    }

    // Public functions to control defender
    public void EnableDefender()
    {
        isDefenderActive = true;
        StartDetection();
    }

    public void DisableDefender()
    {
        isDefenderActive = false;
        StopDetection();

        // Stop shooting and reset state
        GetComponent<Weapon>().canShoot = false;
        isActive = false;

        // Return gun head to original position
        gunHead.localRotation = Quaternion.Euler(originalRotation);
    }

    public void ToggleDefender()
    {
        if (isDefenderActive)
        {
            DisableDefender();
        }
        else
        {
            EnableDefender();
        }
    }

    private void StartDetection()
    {
        if (detectionCoroutine == null)
        {
            detectionCoroutine = StartCoroutine(DetectionLoop());
        }
    }

    private void StopDetection()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
            detectionCoroutine = null;
        }
    }

    IEnumerator DetectionLoop()
    {
        while (isDefenderActive)
        {
            target = FindClosestEnemy();

            if (target)
            {
                if (Vector3.Distance(transform.position, target.position) <= shootingDistance)
                {
                    GetComponent<Weapon>().canShoot = true;
                    isActive = true;
                }
                else
                {
                    GetComponent<Weapon>().canShoot = false;
                    isActive = false;
                }
            }
            else
            {
                GetComponent<Weapon>().canShoot = false;
                isActive = false;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(targetTag);
        if (gos.Length == 0)
            return null;

        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        GameObject closest = null;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest?.transform;
    }
}