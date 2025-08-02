using UnityEngine;
using TMPro;
using System.Collections;



public class DamageText : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI damageText;
    public float lifetime = 2f;
    public float floatSpeed = 50f;

    [Header("Animation Settings")]
    public float punchScale = 1.5f; // Seberapa besar scale maksimal
    public float punchDuration = 0.3f; // Durasi punch effect
    public float countUpDuration = 0.5f; // Durasi counting dari 0 ke damage

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color headshotColor = Color.red;
    public Color criticalColor = Color.yellow;

    private Camera playerCamera;
    private Vector3 worldPosition;
    private RectTransform rectTransform;
    private float targetDamage;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();
    }

    public void Initialize(Vector3 worldPos, float damage, DamageType damageType, Camera camera)
    {
        worldPosition = worldPos;
        playerCamera = camera;
        targetDamage = damage;
        Vector3 screenPos = playerCamera.WorldToScreenPoint(worldPosition);
        rectTransform.position = screenPos;

        // Set color and size based on damage type
        switch (damageType)
        {
            case DamageType.Headshot:
                damageText.color = headshotColor;
                damageText.fontSize = 48f;
                punchScale = 2f; // Headshot lebih besar
                break;
            case DamageType.Critical:
                damageText.color = criticalColor;
                damageText.fontSize = 44f;
                punchScale = 1.8f;
                break;
            default:
                damageText.color = normalColor;
                damageText.fontSize = 36f;
                punchScale = 1.5f;
                break;
        }

        // Start dengan scale kecil
        transform.localScale = Vector3.zero;
        damageText.text = "0";

        StartCoroutine(DamageAnimation());
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Convert world position to screen position
        Vector3 screenPos = playerCamera.WorldToScreenPoint(worldPosition);

        // Check if behind camera
        if (screenPos.z < 0)
        {
            Destroy(gameObject);
            return;
        }

        rectTransform.position = screenPos;
    }

    IEnumerator DamageAnimation()
    {
        float elapsedTime = 0f;
        Vector3 startWorldPos = worldPosition;

        // PHASE 1: Punch Scale + Count Up
        while (elapsedTime < punchDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / punchDuration;

            // Scale animation (0 -> punchScale -> 1)
            float scaleValue;
            if (progress < 0.6f)
            {
                // Scale up ke punch scale
                scaleValue = Mathf.Lerp(0f, punchScale, progress / 0.6f);
            }
            else
            {
                // Scale down ke normal
                scaleValue = Mathf.Lerp(punchScale, 1f, (progress - 0.6f) / 0.4f);
            }
            transform.localScale = Vector3.one * scaleValue;

            // Count up damage number
            if (elapsedTime < countUpDuration)
            {
                float currentDamage = Mathf.Lerp(0f, targetDamage, elapsedTime / countUpDuration);
                damageText.text = Mathf.RoundToInt(currentDamage).ToString();
            }
            else
            {
                damageText.text = Mathf.RoundToInt(targetDamage).ToString();
            }

            yield return null;
        }

        // Pastikan nilai final
        transform.localScale = Vector3.one;
        damageText.text = Mathf.RoundToInt(targetDamage).ToString();

        // PHASE 2: Float and Fade
        elapsedTime = 0f;
        Color startColor = damageText.color;

        while (elapsedTime < lifetime - punchDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (lifetime - punchDuration);

            // Float upward
            worldPosition = startWorldPos + Vector3.up * (floatSpeed * elapsedTime * Time.deltaTime);

            // Fade out
            Color currentColor = startColor;
            currentColor.a = Mathf.Lerp(1f, 0f, progress);
            damageText.color = currentColor;

            yield return null;
        }

        Destroy(gameObject);
    }
}
