using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance;
    [Header("Text Objects")]
    public GameObject playerHudObj;
    public TextMeshProUGUI lifeText, dashText, nukeText, multiplierText, scoreText;
    public Image multiplierBar, ammoBar, timerBar;
    public Animator blackScreen;
    public Animator levelEndAnimator;
    public TextMeshProUGUI levelText;
    public List<UIPointer> pointers;
    [Header("Gameover Objects")]
    public Animator overAnimator;
    public TextMeshProUGUI finalScore, kills, skulls, gems, run, worldLevel;
    public List<Image> coloredUIElements;
    public GameObject pauseScreen;
    public Transform dropTextFolder;
    public GameObject dropTextObject;
    public bool isPaused = false;
    private int displayedScore = 0;
    private Coroutine scoreRoutine;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        // Pauses the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            PlayerManager.Instance.TogglePause(isPaused);
            AudioListener.pause = isPaused; // Pauses the music too
            pauseScreen.SetActive(isPaused);
        }
    }
    public void UnPause()
    {
        isPaused = false;
        PlayerManager.Instance.TogglePause(isPaused);
        AudioListener.pause = isPaused;
        pauseScreen.SetActive(isPaused);
    }

    /// <summary>
    /// Initializes all the stat texts for the UI
    /// </summary>
    /// <param name="lives"></param>
    /// <param name="dashes"></param>
    /// <param name="nukes"></param>
    /// <param name="multiplier"></param>
    public void InitStats(int lives, int dashes, int nukes, int multiplier)
    {
        UpdateText(0, lives);
        UpdateText(1, dashes);
        UpdateText(2, nukes);
        UpdateText(3, multiplier);
        UpdateText(4, 0);

        // Multiplier bar is set to 0
        UpdateBar(0, 0f, 100f);

        // Sets the UI elements to the player's color
        foreach (Image img in coloredUIElements)
        {
            float alpha = img.color.a;
            Color newColor = PlayerManager.Instance.GetColor();
            newColor.a = alpha;
            img.color = newColor;
        }
    }

    /// <summary>
    /// Toggles the black screen
    /// </summary>
    /// <param name="on"></param>
    public void ToggleBlackScreen(bool on)
    {
        // Debug.Log("Toggle Black Screen: " + on);
        blackScreen.SetBool("FadeIn", on);
    }

    /// <summary>
    /// Update the text. 0-life 1-dash 2-nuke 3-multiplier 4-score
    /// </summary>
    /// <param name="textNum"></param>
    /// <param name="val"></param>
    public void UpdateText(int textNum, int val)
    {
        switch (textNum)
        {
            case 0:
                lifeText.text = val.ToString();
                break;
            case 1:
                dashText.text = val.ToString();
                break;
            case 2:
                nukeText.text = val.ToString();
                break;
            case 3:
                multiplierText.text = "x" + val;
                break;
            case 4:
                if (scoreRoutine != null)
                    StopCoroutine(scoreRoutine);

                scoreRoutine = StartCoroutine(LerpScore(val, .25f));

                break;
            default:
                Debug.Log("Invalid Text Index");
                break;
        }
    }

    /// <summary>
    /// Interpolates the score
    /// </summary>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator LerpScore(int target, float duration)
    {
        int start = displayedScore;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(start, target, t));
            scoreText.text = displayedScore.ToString();
            yield return null;
        }

        // Snap to final value
        displayedScore = target;
        scoreText.text = target.ToString();
    }
    /// <summary>
    /// Update the bar fill amount given the values. 0-multi 1-ammo 2-timer
    /// </summary>
    /// <param name="barNum"></param>
    /// <param name="current"></param>
    /// <param name="max"></param>
    public void UpdateBar(int barNum, float current, float max)
    {
        float val = current / max;
        switch (barNum)
        {
            case 0:
                multiplierBar.fillAmount = val;
                break;
            case 1:
                ammoBar.fillAmount = val;
                break;
            case 2:
                timerBar.fillAmount = val;
                break;
            default:
                Debug.Log("Invalid Bar Index");
                break;
        }
    }

    /// <summary>
    /// Changes the bar color
    /// </summary>
    /// <param name="barNum"></param>
    /// <param name="color"></param>
    public void UpdateBarColor(int barNum, Color color)
    {
        switch (barNum)
        {
            case 0:
                multiplierBar.color = color;
                break;
            case 1:
                ammoBar.color = color;
                break;
            case 2:
                timerBar.color = color;
                break;
            default:
                Debug.Log("Invalid Bar Index");
                break;
        }
    }

    /// <summary>
    /// Shows level text when completing a level
    /// </summary>
    /// <param name="levelName"></param>
    /// <param name="levelNum"></param>
    public void AdvanceLevel(string levelName, int levelNum)
    {
        if (levelName == "hide")
        {
            levelEndAnimator.SetTrigger("Hide");
        }
        else
        {
            levelText.text = levelName + " level " + levelNum + " completed!";
            levelEndAnimator.SetTrigger("Show");
        }
    }

    /// <summary>
    /// Shows the world name when completing a challenge world
    /// </summary>
    /// <param name="levelName"></param>
    public void AdvanceLevel(string levelName)
    {
        if (levelName == "hide")
        {
            levelEndAnimator.SetTrigger("Hide");
        }
        else
        {
            levelText.text = levelName + " completed!";
            levelEndAnimator.SetTrigger("Show");
        }
    }

    /// <summary>
    /// Assigns the pointers to a target
    /// </summary>
    /// <param name="index"></param>
    /// <param name="target"></param>
    public void AssignPointers(int index, Transform target)
    {
        UIPointer pointer = pointers[index];
        pointer.gameObject.SetActive(true);
        pointer.target = target;
        pointer.isTargeting = true;
    }

    /// <summary>
    /// Turns off the pointers
    /// </summary>
    public void DisablePointers()
    {
        foreach (UIPointer p in pointers)
        {
            p.isTargeting = false;
            p.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the player stats at the end of the game
    /// </summary>
    /// <param name="newData"></param>
    /// <returns></returns>
    public PlayerData GameOverStats(PlayerData newData)
    {
        PlayerData oldData = SaveSystem.LoadPlayer(newData._name);
        playerHudObj.SetActive(false);
        finalScore.text = newData.currentScore.ToString();
        kills.text = newData.kills.ToString();
        skulls.text = newData.skulls.ToString();
        gems.text = newData.gems.ToString();
        run.text = "Attempt " + newData.attempt;

        // If it's a challenge world, display different text
        if (WorldManager.Instance.isChallenge)
        {
            worldLevel.text = "Challenge World: " + newData.world;
            newData.level = 5;
        }
        else
            worldLevel.text = newData.world + " level " + newData.level;

        overAnimator.SetTrigger("Play");

        // High score + Furthest Run
        if (oldData.highScore < newData.currentScore && oldData.highestWorld < newData.worldIndex)
        {
            newData.highScore = newData.currentScore;
            newData.highestWorld = newData.worldIndex;
            newData.world = WorldManager.Instance.worldName;
            overAnimator.SetBool("Highscore", true);
            overAnimator.SetBool("Furthest", true);
            return newData;
        }

        // Highscore
        if (oldData.highScore < newData.currentScore)
        {
            newData.highScore = newData.currentScore;
            overAnimator.SetBool("Highscore", true);
            return newData;
        }

        // Furthest Run
        if (oldData.highestWorld < newData.worldIndex)
        {
            newData.highestWorld = newData.worldIndex;
            newData.world = WorldManager.Instance.worldName;
            overAnimator.SetBool("Furthest", true);
            return newData;
        }

        oldData.attempt = newData.attempt;
        return oldData;
    }

    /// <summary>
    /// Text that appears at the bottom left of the screen showing what powerup was picked up
    /// </summary>
    /// <param name="text"></param>
    public void DropText(string text)
    {
        Vector2 pos = new Vector2(Random.Range(15f, 50f), Random.Range(15f, 35f));
        GameObject obj = Instantiate(dropTextObject, pos, Quaternion.identity, dropTextFolder);
        TextMeshProUGUI t = obj.GetComponentInChildren<TextMeshProUGUI>();
        t.text = " + " + text;
    }
}
