using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public bool waitForInput = true;
    public GameObject loadingMenu;
    public Slider loadingBar;
    public TextMeshProUGUI loadPromptText;

    private bool isReadyToActivate = false;

    public void StartLoading(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingMenu.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            loadingBar.value = operation.progress;
            yield return null;
        }

        loadingBar.value = 1f;
        loadPromptText.text = waitForInput ? "Press any key to continue..." : "Loading...";

        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.anyKeyDown);
        }

        operation.allowSceneActivation = true;
    }
}
