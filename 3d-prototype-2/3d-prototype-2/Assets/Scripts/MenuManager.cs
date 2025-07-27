using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [Header("UI Elements")]
    public Image loadingBar;
    [Header("Scenes")]
    public SceneField universal;
    public List<SceneField> worldScenes;
    private List<AsyncOperation> scenesToload = new List<AsyncOperation>();
    public int prevIndex = -1;
    void Awake()
    {
        Instance = this;

        loadingBar.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        // scenesToload.Add(SceneManager.LoadSceneAsync(universal));
    }

    public void OpenNewScene(int index)
    {
        if (prevIndex != -1)
        {
            scenesToload.Clear();

            SceneManager.UnloadSceneAsync(worldScenes[prevIndex]);
        }
        scenesToload.Add(SceneManager.LoadSceneAsync(worldScenes[index], LoadSceneMode.Additive));
        StartCoroutine(ProgressLoadingBar(index));

        prevIndex = index;
    }

private IEnumerator ProgressLoadingBar(int index)
{
    loadingBar.gameObject.SetActive(true);

    float totalProgress = 0f;

    while (true)
    {
        // Sum all loading progress
        for (int i = 0; i < scenesToload.Count; i++)
        {
            totalProgress += scenesToload[i].progress;
        }

        // Normalize and apply to UI
        loadingBar.fillAmount = totalProgress / scenesToload.Count;

        // If all are done, break
        bool allDone = true;
        foreach (var op in scenesToload)
        {
            if (!op.isDone)
            {
                allDone = false;
                break;
            }
        }

        if (allDone) break;

        yield return null; // Wait 1 frame, smooth update
    }

    GameManager.Instance.player.DisableInput();
    GameManager.Instance.PlayCutscene(index);
    loadingBar.gameObject.SetActive(false);
}
}
