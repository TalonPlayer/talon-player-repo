using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Animator menuAnimator;
    public SceneField universalGameplay;
    public PlayerModel playerPrefab;
    public PlayerModel modelInstance;
    public List<string> players;
    public List<PlayerData> playerDatas;
    public List<Transform> modelPoints;
    public List<Button> saveButtons;
    public List<TextMeshProUGUI> saveButtonTexts;
    public List<TextMeshPro> infoTexts;
    public Transform customizePoint;
    public Transform defaultPoint;
    public Button savesButton;
    public Button continueButton;
    public Renderer door;
    public List<Image> uiElements;
    [SerializeField] private PlayerInfo playerInfo;
    public string selectedColorCode;
    void Start()
    {
        players = SaveSystem.LoadPlayerList();
        foreach (string p in players)
        {
            playerDatas.Add(SaveSystem.LoadPlayer(p));
        }

        for (int i = 0; i < players.Count; i++)
        {
            PlayerData data = playerDatas[i];
            Transform point = modelPoints[i];
            PlayerModel p = Instantiate(playerPrefab,
                point.position,
                point.rotation,
                point);
            p.ChangeColor(data.colorCode);
            p.SetRandom();


            saveButtons[i].interactable = true;

            Image img = saveButtons[i].GetComponent<Image>();
            float alpha = img.color.a;
            Color newColor = GetColorByCode(data.colorCode);
            newColor.a = alpha;
            img.color = newColor;

            saveButtonTexts[i].text = data._name;

            infoTexts[i].text =
                "\nAttempts: " + data.attempt +
                "\nHighscore: " + data.highScore +
                "\n" + data.world + " level " + data.level +
                "\n" + data.kills + " kills" +
                "\n" + data.skulls + " skulls " +
                "\n" + data.gems + " gems";
        }

        Invoke(nameof(EnableContinue), 5.0f);
    }

    void EnableContinue()
    {
        menuAnimator.enabled = false;

        if (players.Count == 0) savesButton.interactable = false;
        else savesButton.interactable = true;

        savesButton.interactable = players.Count != 0;
        menuAnimator.enabled = true;

    }

    public void SelectColorByCode(string code)
    {
        menuAnimator.enabled = false;
        selectedColorCode = code;
        modelInstance.ChangeColor(code);
        Color color = GetColorByCode(code);

        foreach (Image image in uiElements)
        {
            float alpha = image.color.a;
            Color newColor = color;
            newColor.a = alpha;
            image.color = newColor;
        }

        door.material.color = color;
        menuAnimator.enabled = true;
    }

    public Color GetColorByCode(string code)
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(code, out color))
        {
            Debug.LogWarning("Invalid Hex Code");
            return new Color();
        }
        return color;
    }

    void Update()
    {

    }

    public void MenuGoTo(string name)
    {
        menuAnimator.SetTrigger("GoTo" + name);

        switch (name)
        {
            case "Start":
                StartCoroutine(OpenGame());
                break;
            case "Start2":
                StartCoroutine(OpenGame());
                break;
            case "Continue":
                break;
            case "Customize":
                Invoke(nameof(DelayCustomize), 1f);
                break;
            case "Back":
                Invoke(nameof(DelayTeleport), 1f);
                break;
            case "HowToPlay":
                Debug.Log("How To Play");
                break;
            case "Options":
                Debug.Log("Options");
                break;
            case "Credits":
                Debug.Log("Credits");
                break;
            case "Exit":
                Debug.Log("Exit");
                break;
        }
    }
    void DelayCustomize()
    {
        modelInstance.transform.position = customizePoint.position;
        modelInstance.transform.rotation = customizePoint.rotation;
    }
    void DelayTeleport()
    {
        modelInstance.transform.position = defaultPoint.position;
        modelInstance.transform.rotation = defaultPoint.rotation;
    }
    public IEnumerator OpenGame()
    {
        yield return new WaitForSeconds(7f);

        SceneManager.LoadScene(universalGameplay);
    }

    public void SelectCharacter(int index)
    {
        PlayerData data = playerDatas[index];
        SelectColorByCode(data.colorCode);
        continueButton.interactable = true;
        SaveSystem.selectedPlayerName = data._name;
    }

    public void CreateCharacter(string name)
    {
        PlayerData data = new(name, selectedColorCode);
        playerInfo.CopyPlayer(data);
        SaveSystem.SavePlayer(playerInfo);
        SaveSystem.selectedPlayerName = data._name;

    }
}
