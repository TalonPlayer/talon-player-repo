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
    private int displayedScore = 0;
    private Coroutine scoreRoutine;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ToggleBlackScreen(false);
    }
    public void InitStats(int lives, int dashes, int nukes, int multiplier)
    {
        UpdateText(0, lives);
        UpdateText(1, dashes);
        UpdateText(2, nukes);
        UpdateText(3, multiplier);
        UpdateText(4, 0);

        UpdateBar(0, 0f, 100f);
    }

    public void ToggleBlackScreen(bool on)
    {
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
}
