using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;
    [SerializeField] private TextMeshProUGUI buttonLetter;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private Animator counterAnimator;
    public Animator canvasAnimator;
    public Animator deathAnimator;
    public List<GameObject> hudObjects = new List<GameObject>();
    public GameObject blackScreen;


    [Header("Health Objects")]
    [SerializeField] private Image outerImg;
    [SerializeField] private Image pointerImg;
    [SerializeField] private Image glassImg;
    public Image comboImg;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Health Colors")]
    // element 0 is dark red
    // element 7 is bright green
    public List<Color> colors = new List<Color>();
    public List<Color> textColor = new List<Color>();

    void Awake()
    {
        Instance = this;
    }
    public void OpenIntButton(Interactable inter)
    {
        buttonLetter.text = inter.letter;
        buttonText.text = inter.message;
        buttonAnimator.SetBool("IsOpen", true);
    }

    public void CloseIntButton()
    {
        buttonAnimator.SetBool("IsOpen", false);
    }

    public void OpenScene()
    {
        canvasAnimator.SetBool("IsOpen", true);
    }

    public void CloseScene()
    {
        canvasAnimator.SetBool("IsOpen", false);
    }
    public void ChangeHealth(int currentHealth)
    {
        if (currentHealth <= 0)
        {
            PlayDeathAnim(true);
            healthText.text = "" + currentHealth;
            int index = 0;
            float targetAngle = currentHealth * 45f;
            StopAllCoroutines();
            StartCoroutine(LerpHealthUI(index, targetAngle));
        }
        else
        {
            healthText.text = "" + currentHealth;
            int index = Mathf.Clamp(currentHealth - 1, 0, 8);
            float targetAngle = currentHealth * 45f;
            StopAllCoroutines();
            StartCoroutine(LerpHealthUI(index, targetAngle));
        }

    }

    private IEnumerator LerpHealthUI(int index, float targetAngle)
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Quaternion startRotation = pointerImg.rectTransform.localRotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, -targetAngle);

        Color startOuter = outerImg.color;
        Color startPointer = pointerImg.color;
        Color startGlass = glassImg.color;
        Color startHealth = healthText.color;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            pointerImg.rectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            outerImg.color = Color.Lerp(startOuter, colors[index], t);
            pointerImg.color = Color.Lerp(startPointer, colors[index], t);
            glassImg.color = Color.Lerp(startGlass, colors[index], t);
            healthText.color = Color.Lerp(startHealth, textColor[index], t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final snap to ensure precision
        pointerImg.rectTransform.localRotation = endRotation;
        outerImg.color = colors[index];
        pointerImg.color = colors[index];
        glassImg.color = colors[index];
        healthText.color = textColor[index];
    }

    public void TurnOn(bool isOn)
    {
        foreach (GameObject h in hudObjects)
        {
            h.SetActive(isOn);
        }

        blackScreen.SetActive(!isOn);
    }

    public void PlayDeathAnim(bool isDead)
    {
        deathAnimator.SetBool("IsDead", isDead);
    }
    
    public void UpdateCombo(float current){
        comboImg.fillAmount = current / 8;
    }
}
