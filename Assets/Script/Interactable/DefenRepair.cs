using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenRepair : Interactable
{
    [Header("Weapon Settings")]
    public float weaponDuration = 30f;
    public bool startBroken = true;

    [Header("Repair Settings")]
    public float repairTime = 3f;
    public string brokenPrompt = "Press E to Repair";
    public string repairingPrompt = "Repairing...";
    public string activePrompt = "Weapon Active";

    [Header("UI Elements")]
    public GameObject progressUI;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Script References")]
    [SerializeField] private Defender_AI defenderAI;
    [SerializeField] private Weapon weapon;

    private bool isBroken;
    private bool isRepairing;
    private Coroutine weaponTimer;
    private Coroutine repairCoroutine;

    void Start()
    {
        isBroken = startBroken;
        UpdatePrompt();
        InitializeUI();


        if (isBroken)
        {
            OnWeaponBroken();
        }
        else
        {
            OnWeaponRepaired();
            StartWeaponTimer();
        }
    }

    void InitializeUI()
    {
        if (progressUI) progressUI.SetActive(false);
        if (progressBar) progressBar.value = 0f;
    }

    protected override void Interact()
    {
        if (isBroken && !isRepairing)
        {
            repairCoroutine = StartCoroutine(RepairWeapon());
        }

        base.Interact();
    }

    IEnumerator RepairWeapon()
    {
        isRepairing = true;
        CanInteract = false;
        UpdatePrompt();

        if (progressUI) progressUI.SetActive(true);
        UpdateProgressText("Repairing...");

        float elapsed = 0f;
        while (elapsed < repairTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / repairTime;

            if (progressBar) progressBar.value = progress;

            yield return null;
        }

        isBroken = false;
        isRepairing = false;
        CanInteract = true;
        UpdatePrompt();

        StartWeaponTimer();
        OnWeaponRepaired();
    }

    void StartWeaponTimer()
    {
        if (weaponTimer != null)
            StopCoroutine(weaponTimer);

        weaponTimer = StartCoroutine(WeaponDurationTimer());
    }

    IEnumerator WeaponDurationTimer()
    {
        if (progressUI) progressUI.SetActive(true);
        UpdateProgressText("Active");

        float elapsed = 0f;
        while (elapsed < weaponDuration)
        {
            elapsed += Time.deltaTime;
            float remaining = 1f - (elapsed / weaponDuration);

            if (progressBar) progressBar.value = remaining;

            yield return null;
        }

        if (progressUI) progressUI.SetActive(false);
        BreakWeapon();
    }

    void BreakWeapon()
    {
        isBroken = true;
        CanInteract = true;
        UpdatePrompt();

        if (progressUI) progressUI.SetActive(false);

        if (weaponTimer != null)
        {
            StopCoroutine(weaponTimer);
            weaponTimer = null;
        }

        OnWeaponBroken();
    }

    void UpdatePrompt()
    {
        if (isRepairing)
            PromptMessage = repairingPrompt;
        else if (isBroken)
            PromptMessage = brokenPrompt;
        else
            PromptMessage = activePrompt;
    }

    public void StopRepair()
    {
        if (repairCoroutine != null)
        {
            StopCoroutine(repairCoroutine);
            repairCoroutine = null;
            isRepairing = false;
            CanInteract = true;
            UpdatePrompt();

            if (progressUI) progressUI.SetActive(false);
            if (progressBar) progressBar.value = 0f;
        }
    }

    void UpdateProgressText(string status)
    {
        if (progressText) progressText.text = status;
    }


    protected virtual void OnWeaponRepaired()
    {
        Debug.Log("Weapon Repaired - Enabling Defender");
        if (defenderAI) defenderAI.EnableDefender();
        if (weapon) weapon.enabled = true;
    }

    protected virtual void OnWeaponBroken()
    {

        if (defenderAI) defenderAI.DisableDefender();
        if (weapon) weapon.enabled = false;
    }


    public bool IsBroken => isBroken;
    public bool IsRepairing => isRepairing;
    public bool IsActive => !isBroken && !isRepairing;

    void OnDestroy()
    {
        if (weaponTimer != null) StopCoroutine(weaponTimer);
        if (repairCoroutine != null) StopCoroutine(repairCoroutine);
    }
}