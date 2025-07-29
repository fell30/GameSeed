using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum HealthType
{
    Player,
    Tower
}

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public HealthType healthType = HealthType.Player;
    public float maxHealth = 100f;
    public bool startWithFullHealth = true;

    [Header("UI Elements")]
    public Slider healthSlider;
    public bool hideWhenFull = false;

    [Header("Visual Effects")]
    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public Image healthFill;

    [Header("Regeneration (Optional)")]
    public bool canRegenerate = false;
    public float regenRate = 5f; // health per second
    public float regenDelay = 3f; // delay after taking damage

    [Header("Events")]
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnHealthFull;
    public UnityEvent OnHealthLow; // Below 25%

    // Private variables
    private float currentHealth;
    private bool isDead = false;
    private Coroutine regenCoroutine;
    private float lastDamageTime;

    // Properties
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsDead => isDead;
    public bool IsFullHealth => currentHealth >= maxHealth;
    public bool IsLowHealth => HealthPercentage <= 0.25f;

    void Start()
    {
        InitializeHealth();
        SetupUI();

        if (canRegenerate)
            StartRegeneration();
    }

    void InitializeHealth()
    {
        currentHealth = startWithFullHealth ? maxHealth : 1f;
        UpdateHealthUI();
    }

    void SetupUI()
    {
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;

            if (hideWhenFull && IsFullHealth)
                healthSlider.gameObject.SetActive(false);
        }

        UpdateHealthColor();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        lastDamageTime = Time.time;

        Debug.Log($"{healthType} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        UpdateHealthUI();
        OnHealthChanged?.Invoke();

        if (IsLowHealth && !isDead)
            OnHealthLow?.Invoke();

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead || healAmount <= 0) return;

        float oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);

        Debug.Log($"{healthType} healed {healAmount}. Health: {currentHealth}/{maxHealth}");

        UpdateHealthUI();
        OnHealthChanged?.Invoke();

        if (IsFullHealth && oldHealth < maxHealth)
            OnHealthFull?.Invoke();
    }

    public void SetHealth(float newHealth)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthUI();
        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    public void FullHeal()
    {
        Heal(maxHealth);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        Debug.Log($"{healthType} has died!");

        UpdateHealthUI();
        OnDeath?.Invoke();

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        HandleDeath();
    }

    void HandleDeath()
    {
        switch (healthType)
        {
            case HealthType.Player:
                // Player specific death logic
                Debug.Log("Game Over!");
                break;

            case HealthType.Tower:
                // Tower specific death logic
                Debug.Log("Tower Destroyed!");
                break;
        }
    }

    public void Revive(float healthAmount = -1)
    {
        if (!isDead) return;

        isDead = false;
        currentHealth = healthAmount > 0 ? Mathf.Clamp(healthAmount, 1, maxHealth) : maxHealth;

        UpdateHealthUI();
        OnHealthChanged?.Invoke();

        if (canRegenerate)
            StartRegeneration();

        Debug.Log($"{healthType} revived with {currentHealth} health!");
    }

    void UpdateHealthUI()
    {
        if (!healthSlider) return;

        healthSlider.value = currentHealth;

        // Show/hide UI based on settings
        if (hideWhenFull)
        {
            healthSlider.gameObject.SetActive(!IsFullHealth || isDead);
        }

        UpdateHealthColor();
    }

    void UpdateHealthColor()
    {
        if (!healthFill) return;

        float percentage = HealthPercentage;

        if (percentage > 0.6f)
            healthFill.color = Color.Lerp(midHealthColor, fullHealthColor, (percentage - 0.6f) / 0.4f);
        else if (percentage > 0.25f)
            healthFill.color = Color.Lerp(lowHealthColor, midHealthColor, (percentage - 0.25f) / 0.35f);
        else
            healthFill.color = lowHealthColor;
    }

    void StartRegeneration()
    {
        if (regenCoroutine == null)
            regenCoroutine = StartCoroutine(RegenerationCoroutine());
    }

    IEnumerator RegenerationCoroutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.1f);

            // Only regenerate if enough time has passed since last damage
            if (Time.time - lastDamageTime >= regenDelay && currentHealth < maxHealth)
            {
                Heal(regenRate * 0.1f); // 0.1f because we check every 0.1 seconds
            }
        }
    }

    // Public utility methods
    public void SetMaxHealth(float newMaxHealth)
    {
        float percentage = HealthPercentage;
        maxHealth = newMaxHealth;
        currentHealth = maxHealth * percentage;

        if (healthSlider)
            healthSlider.maxValue = maxHealth;

        UpdateHealthUI();
    }

    public void AddMaxHealth(float additionalHealth)
    {
        SetMaxHealth(maxHealth + additionalHealth);
    }

    void OnDestroy()
    {
        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);
    }


    [ContextMenu("Take 10 Damage")]
    void DebugTakeDamage() => TakeDamage(10f);

    [ContextMenu("Heal 20")]
    void DebugHeal() => Heal(20f);

    [ContextMenu("Full Heal")]
    void DebugFullHeal() => FullHeal();

    [ContextMenu("Kill")]
    void DebugKill() => TakeDamage(maxHealth);
}