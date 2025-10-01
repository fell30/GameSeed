using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public GameObject WinGamePanel;
    public GameObject LoseGamePanel;
    public GameObject PauseMenu;
    public PlayerMotor playerMotor;
    public PlayerLook playerLook;
    public spawnZombie spawnZombie;
    public Pistol pistol;


    void Start()
    {
        playerLook.enabled = false;
        playerMotor.enabled = false;
        spawnZombie.enabled = false;
        pistol.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    public void TogglePauseMenu()
    {
        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerLook.enabled = true;
        playerMotor.enabled = true;
        spawnZombie.enabled = true;
        pistol.enabled = true;


    }
    public void ShowWinGamePanel()
    {
        WinGamePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ShowLoseGamePanel()
    {
        LoseGamePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
