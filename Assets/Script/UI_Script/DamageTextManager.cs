using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DamageTextManager : MonoBehaviour
{
    [Header("Damage Text Settings")]
    public GameObject damageTextPrefab;
    public Transform canvasTransform;
    public Camera playerCamera;

    public static DamageTextManager Instance;

    void Awake()
    {
        Instance = this;
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    public void ShowDamageText(Vector3 worldPosition, float damage, DamageType damageType = DamageType.Normal)
    {
        GameObject damageObj = Instantiate(damageTextPrefab, canvasTransform);
        DamageText damageText = damageObj.GetComponent<DamageText>();
        damageText.Initialize(worldPosition, damage, damageType, playerCamera);
    }
}

public enum DamageType
{
    Normal,
    Headshot,
    Critical
}