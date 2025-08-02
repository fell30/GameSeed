using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private string selectedLevelName;
    public TextMeshProUGUI percentText;
    public Slider LoadingScreen;

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
            StartCoroutine(LoadLevelAsync(selectedLevelName));

        }
        else
        {
            StartCoroutine(LoadLevelAsync("Level_1"));

        }
    }
    IEnumerator LoadLevelAsync(string levelName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (LoadingScreen != null)
                LoadingScreen.value = progress;

            if (percentText != null)
                percentText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";

            timer += Time.deltaTime;
            yield return null;
        }

        // Pastikan loading minimal 2 detik
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Finalize loading bar
        if (LoadingScreen != null)
            LoadingScreen.value = 1f;

        if (percentText != null)
            percentText.text = "Loading... 100%";

        yield return new WaitForSeconds(0.5f);
        operation.allowSceneActivation = true;
    }
    public void QuitGame()
    {

        Application.Quit();

    }
}
