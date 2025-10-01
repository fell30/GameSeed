using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    [Header("Health")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealtBar;
    public Image backHealtBar;
    public TextMeshProUGUI healthText;
    [Header("DamageOverlay")]
    public Image Overlay;
    public float duration;
    public float FadeSpeed;
    private float durationTimer;
    public UI_Manager uiManager;
    private bool isDead = false;


    void Start()
    {
        health = maxHealth;
        Overlay.color = new Color(Overlay.color.r, Overlay.color.g, Overlay.color.b, 0);

    }
    void Update()
    {
        healthText.text = health.ToString();
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (Overlay.color.a > 0)
        {
            if (health < 30)
                return;
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = Overlay.color.a;
                tempAlpha -= Time.deltaTime * FadeSpeed;
                Overlay.color = new Color(Overlay.color.r, Overlay.color.g, Overlay.color.b, tempAlpha);

            }
        }

    }

    public void UpdateHealthUI()
    {

        float fillF = frontHealtBar.fillAmount;
        float fillB = backHealtBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction)
        {
            frontHealtBar.fillAmount = hFraction;
            backHealtBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float precentComplete = lerpTimer / chipSpeed;
            precentComplete = precentComplete * precentComplete;
            backHealtBar.fillAmount = Mathf.Lerp(fillB, hFraction, precentComplete);
        }
        if (fillF < hFraction)
        {
            backHealtBar.color = Color.green;
            backHealtBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float precentComplete = lerpTimer / chipSpeed;
            precentComplete = precentComplete * precentComplete;
            frontHealtBar.fillAmount = Mathf.Lerp(fillF, backHealtBar.fillAmount, precentComplete);

        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0f;
        Overlay.color = new Color(Overlay.color.r, Overlay.color.g, Overlay.color.b, 1);
        if (health <= 0 && !isDead)
        {
            isDead = true;
            uiManager.ShowLoseGamePanel();
            Debug.Log("Player is dead");
        }
    }
    public void RestoreHealth(float HealAmount)
    {
        health += HealAmount;

        lerpTimer = 0f;

    }
}
