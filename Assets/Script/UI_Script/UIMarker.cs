using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMarker : MonoBehaviour
{
    public Image img;
    public Transform target;
    public TextMeshProUGUI meter;
    public Vector3 offset;

    [Header("Optional Settings")]
    public bool rotateToTarget = false;
    public bool fadeByDistance = false;
    public float hideWhenCloserThan = 2f;

    private void Update()
    {
        if (target == null || img == null || meter == null)
            return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);

        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;
        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        // Jika target berada di belakang kamera
        if (screenPos.z < 0)
        {
            screenPos.x = screenPos.x < Screen.width / 2 ? maxX : minX;
        }

        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        img.transform.position = screenPos;
        meter.transform.position = screenPos + new Vector3(0, -25f, 0); // Offset ke bawah

        float distance = Vector3.Distance(target.position, Camera.main.transform.position);
        meter.text = ((int)distance).ToString() + "m";

        // Sembunyikan jika terlalu dekat
        bool shouldHide = distance < hideWhenCloserThan;
        img.enabled = !shouldHide;
        meter.enabled = !shouldHide;

        // Fade alpha tanpa CanvasGroup
        if (fadeByDistance)
        {
            float fade = Mathf.Clamp01((distance - hideWhenCloserThan) / 10f); // Fade 2–12m

            Color imgColor = img.color;
            imgColor.a = fade;
            img.color = imgColor;

            Color textColor = meter.color;
            textColor.a = fade;
            meter.color = textColor;
        }

        // Rotasi ikon menghadap target
        if (rotateToTarget)
        {
            Vector3 dir = (target.position + offset - Camera.main.transform.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            img.transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}
