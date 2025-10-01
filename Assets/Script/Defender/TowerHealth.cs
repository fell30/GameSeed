using UnityEngine;
using UnityEngine.UI;

public class TowerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Elements")]
    public Slider healthSlider;

    [Header("Audio Setting")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Start()
    {
        // Set health ke maksimum di awal
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        // Setup slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Fungsi untuk mengambil damage
    public void TakeDamage(float damage)
    {
        // Kurangi health
        currentHealth -= damage;

        // Pastikan health tidak kurang dari 0
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Update slider
        UpdateHealthSlider();

        // Debug log untuk testing
        Debug.Log($"Tower took {damage} damage. Current health: {currentHealth}");

        // Cek apakah health sudah 0
        if (currentHealth <= 0)
        {
            OnHealthZero();
        }
    }

    // Fungsi untuk heal (bonus feature)
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthSlider();

        Debug.Log($"Tower healed {healAmount}. Current health: {currentHealth}");
    }

    // Update slider UI
    void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Logic ketika health = 0 (kamu yang isi)
    void OnHealthZero()
    {
        Debug.Log("Tower health reached zero!");

        // TODO: Tambahkan logic kamu di sini
        // Contoh:
        // - Destroy tower
        // - Play death animation
        // - Spawn explosion effect
        // - Update game state
        // - Show game over screen
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        Destroy(gameObject);

        // Sementara ini cuma log
        Debug.Log("Add your zero health logic here!");
    }

    // Getter untuk current health (kalau butuh dari script lain)
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // Getter untuk max health
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Cek apakah tower masih hidup
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    // Reset health ke maksimum
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthSlider();
        Debug.Log("Tower health reset to maximum");
    }
}