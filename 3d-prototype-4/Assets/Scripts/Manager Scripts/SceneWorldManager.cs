using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneWorldManager : MonoBehaviour
{
    public static SceneWorldManager Instance;
    public SceneField universalGameplay;
    public List<SceneField> worldScenes;
    private List<AsyncOperation> scenesToload = new List<AsyncOperation>();
    public int prevIndex = -1;
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Load a new scene and unload the current
    /// </summary>
    /// <param name="index">The index of the next world</param>
    public void TransferToWorld(int index)
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

        float totalProgress = 0f;

        while (true)
        {
            // Sum all loading progress
            for (int i = 0; i < scenesToload.Count; i++)
            {
                totalProgress += scenesToload[i].progress;
            }

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

            yield return new WaitForSeconds(1f); // Wait 1 frame, smooth update
            PlayerManager.Instance.TeleportPlayer(WorldManager.Instance.worldCenter);

            yield return new WaitForSeconds(1f);
            HudManager.Instance.ToggleBlackScreen(false);

            PlayerManager.Instance.SpawnBufferedUnits();
        }

    }
}
