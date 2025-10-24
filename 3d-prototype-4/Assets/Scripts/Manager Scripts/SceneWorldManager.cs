using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneWorldManager : MonoBehaviour
{
    public static SceneWorldManager Instance;
    public SceneField menuScene;
    public SceneField universalGameplay;
    public SceneField slaughterhouse;
    public List<SceneField> worldScenes;
    private List<AsyncOperation> scenesToload = new List<AsyncOperation>();
    public int prevIndex = -1;
    public bool loadedRedRoom = false;
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
            // If you're coming back from Slaughterhouse, unload that instead
            if (loadedRedRoom)
            {
                loadedRedRoom = false;
                SceneManager.UnloadSceneAsync(slaughterhouse);
            }
            else
            {
                scenesToload.Clear();

                EndScene(prevIndex);
            }
        }
        scenesToload.Add(SceneManager.LoadSceneAsync(worldScenes[index], LoadSceneMode.Additive));
        StartCoroutine(ProgressLoadingBar(index));

        prevIndex = index;
    }

    /// <summary>
    /// Transfers the player to Slaughterhouse
    /// </summary>
    public void TransferToRedRoom()
    {
        if (prevIndex != -1)
        {
            scenesToload.Clear();

            EndScene(prevIndex);
        }
        loadedRedRoom = true;
        scenesToload.Add(SceneManager.LoadSceneAsync(slaughterhouse, LoadSceneMode.Additive));
        StartCoroutine(ProgressLoadingBar(0));

    }

    /// <summary>
    /// Ends the scene
    /// </summary>
    /// <param name="index"></param>
    public void EndScene(int index)
    {
        scenesToload.Clear();
        SceneManager.UnloadSceneAsync(worldScenes[index]);
    }

    /// <summary>
    /// Transfers to the menu
    /// </summary>
    public void TransferToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    /// <summary>
    /// Loading bar
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
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

            yield return new WaitForSeconds(2f); // Wait 1 frame, smooth update

            foreach (Player player in PlayerManager.Instance.players)
            {
                player.TeleportPlayer(WorldManager.Instance.worldCenter);
            }

            yield return new WaitForSeconds(2f);
            HudManager.Instance.ToggleBlackScreen(false);

            foreach (Player player in PlayerManager.Instance.players)
            {
                PlayerManager.Instance.SpawnBufferedUnits(player);
            }

        }

    }
}
