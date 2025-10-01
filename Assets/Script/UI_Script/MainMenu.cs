using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Level Selection")]
    private string selectedLevelName;

    [Header("Loading Screen")]
    public TextMeshProUGUI percentText;
    public Slider LoadingScreen;
    public GameObject loadingPanel; // TAMBAHKAN LOADING PANEL

    [Header("Settings")]
    public float minimumLoadingTime = 2f;

    private void Start()
    {
        // PASTIKAN LOADING PANEL TIDAK AKTIF DI AWAL
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    public void EnableMenu(GameObject menu)
    {
        if (menu != null)
            menu.SetActive(true);
    }

    public void DisableMenu(GameObject menu)
    {
        if (menu != null)
            menu.SetActive(false);
    }

    public void SwitchScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        Time.timeScale = 1f;
        StartCoroutine(LoadLevelAsync(sceneName));
    }

    public void SelectLevel(string levelName)
    {
        if (string.IsNullOrEmpty(levelName))
        {
            Debug.LogWarning("Level name is null or empty!");
            return;
        }

        selectedLevelName = levelName;
        Debug.Log("Level selected: " + selectedLevelName);
    }

    public void PlaySelectedLevel()
    {
        string levelToLoad = !string.IsNullOrEmpty(selectedLevelName) ? selectedLevelName : "Level_1";
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    IEnumerator LoadLevelAsync(string levelName)
    {
        // VALIDASI SCENE NAME
        if (string.IsNullOrEmpty(levelName))
        {
            Debug.LogError("Cannot load scene: levelName is null or empty!");
            yield break;
        }

        // PAKSA TIME SCALE KE 1 UNTUK MEMASTIKAN COROUTINE BERJALAN
        Time.timeScale = 1f;

        // AKTIFKAN LOADING PANEL
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        // RESET LOADING UI
        if (LoadingScreen != null)
            LoadingScreen.value = 0f;

        if (percentText != null)
            percentText.text = "Loading... 0%";

        // MULAI ASYNC LOADING
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

        if (operation == null)
        {
            Debug.LogError("Failed to start loading scene: " + levelName);
            yield break;
        }

        operation.allowSceneActivation = false;
        float timer = 0f;

        // LOADING PROGRESS - GUNAKAN UNSCALED TIME
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            UpdateLoadingUI(progress);

            timer += Time.unscaledDeltaTime; // GUNAKAN UNSCALED TIME
            yield return null;
        }

        // PASTIKAN LOADING MINIMAL SESUAI minimumLoadingTime
        while (timer < minimumLoadingTime)
        {
            // UPDATE PROGRESS SECARA VISUAL SAJA
            float fakeProgress = Mathf.Lerp(0.9f, 1f, (timer - 0.9f * minimumLoadingTime) / (minimumLoadingTime - 0.9f * minimumLoadingTime));
            UpdateLoadingUI(Mathf.Clamp01(fakeProgress));

            timer += Time.unscaledDeltaTime; // GUNAKAN UNSCALED TIME
            yield return null;
        }

        // FINALIZE LOADING
        UpdateLoadingUI(1f);

        if (percentText != null)
            percentText.text = "Complete!";

        yield return new WaitForSecondsRealtime(0.5f); // GUNAKAN REALTIME

        // ACTIVATE SCENE
        operation.allowSceneActivation = true;
        Time.timeScale = 1f;
    }

    private void UpdateLoadingUI(float progress)
    {
        if (LoadingScreen != null)
            LoadingScreen.value = progress;

        if (percentText != null)
            percentText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // TAMBAHAN: METHOD UNTUK CEK APAKAH SCENE ADA
    private bool DoesSceneExist(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameFromPath == sceneName)
                return true;
        }
        return false;
    }

    // TAMBAHAN: METHOD UNTUK VALIDASI SCENE SEBELUM LOAD
    public void SwitchSceneWithValidation(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        if (!DoesSceneExist(sceneName))
        {
            Debug.LogError("Scene '" + sceneName + "' does not exist in build settings!");
            return;
        }

        Time.timeScale = 1f;
        StartCoroutine(LoadLevelAsync(sceneName));
    }
}