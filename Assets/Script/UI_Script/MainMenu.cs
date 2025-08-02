using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string selectedLevelName;
    public void EnableMenu(GameObject menu)
    {
        menu.SetActive(true);
    }
    public void DisableMenu(GameObject menu)
    {
        menu.SetActive(false);
    }
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
    public void SelectLevel(string levelName)
    {
        selectedLevelName = levelName;
        Debug.Log("Level selected: " + selectedLevelName);
    }

    public void PlaySelectedLevel()
    {
        if (!string.IsNullOrEmpty(selectedLevelName))
        {
            SceneManager.LoadScene(selectedLevelName);
        }
        else
        {
            Debug.LogWarning("No level selected!");
        }
    }
    public void QuitGame()
    {

        Application.Quit();

    }
}
