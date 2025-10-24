using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Animator menuAnimator;
    public Animator jumpscare;
    public Animator creditsAnimator;
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
    public Button startButton;
    public Button continueButton;
    public Renderer door;
    public List<Image> uiElements;
    [SerializeField] private PlayerInfo playerInfo;
    public string selectedColorCode;
    public int costumeIndex;

    [Header("Toggles")]
    public Toggle tutorialToggle;
    void Awake()
    {
        GlobalSaveSystem.trackAchivement = false;

        // GlobalSaveSystem.LoadOrCreate();
        Cursor.visible = true;

        bool tutorialOn = PlayerPrefs.GetInt("ToggleTutorial", 0) == 0;
        tutorialToggle.isOn = tutorialOn;
        tutorialToggle.onValueChanged.AddListener(SetTutorial);

        players = SaveSystem.LoadPlayerList();
        foreach (string p in players)
        {
            playerDatas.Add(SaveSystem.LoadPlayer(p));
        }

        bool hasSave = players.Contains(PlayerPrefs.GetString("SelectedPlayer"));
        if (!hasSave) PlayerPrefs.GetString("SelectedPlayer", null);
    }
    public void SetTutorial(bool isOn)
    {
        PlayerPrefs.SetInt("ToggleTutorial", isOn ? 0 : 1);
        PlayerPrefs.Save();
    }
    void Start()
    {
        if (players.Count == 0)
        {
            continueButton.interactable = false;
            PlayerPrefs.SetString("SelectedPlayer", null);
        }
        else
        {
            SaveSystem.LoadSelectedPlayerName();

            if (!string.IsNullOrEmpty(SaveSystem.selectedPlayerName))
            {
                PlayerData data = SaveSystem.LoadPlayer(SaveSystem.selectedPlayerName);
                SelectColorByCode(data.colorCode);
                SelectCostume(data.costumeIndex);

                continueButton.interactable = true;
            }
            else
            {
                PlayerData currentData = SaveSystem.LoadPlayer(players[0]);
                SelectColorByCode(currentData.colorCode);
                PlayerPrefs.SetString("SelectedPlayer", currentData._name);
            }
        }
        PlayerPrefs.Save();
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

    public void SelectCostume(int index)
    {
        costumeIndex = index;
        modelInstance.ChangeOutfit(index);
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
        if (Random.Range(0, 101) == 0)
            jumpscare.SetTrigger("Play");
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
                break;
            case "Options":
                break;
            case "Credits":
                creditsAnimator.Play("Credits Scroll");
                break;
            case "Exit":
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
        if (players.Count == 0)
        {
            playerInfo.colorCode = selectedColorCode;
            playerInfo.costumeIndex = costumeIndex;
            SaveSystem.SavePlayer(playerInfo);
            Debug.Log("Menu Player Name: " + playerInfo._name);
            Debug.Log("Menu Player Info Costume Index: " + playerInfo.costumeIndex);
        }
        else
        {
            PlayerData data = SaveSystem.LoadPlayer(SaveSystem.selectedPlayerName);
            playerInfo.CopyPlayer(data);
            data.colorCode = selectedColorCode;
            data.costumeIndex = costumeIndex;
            SaveSystem.SavePlayer(playerInfo);
        }

        SceneManager.LoadScene(universalGameplay);
    }

    public void CreateCharacter(string name)
    {
        PlayerData data = new(name, selectedColorCode, costumeIndex);
        playerInfo.CopyPlayer(data);
        SaveSystem.SavePlayer(playerInfo);
        SaveSystem.selectedPlayerName = data._name;
        Debug.Log("Selected Character: " + data._name);
        PlayerPrefs.SetString("SelectedPlayer", data._name);
        PlayerPrefs.Save();
    }
    public void CreateCharacterLocal()
    {
        SaveSystem.currentPlayers.Clear();
        foreach (PlayerData data in LobbyManager.Instance.players)
        {
            Debug.Log("data: " + data._name + "," + data.colorCode + ", " + data.costumeIndex);
            playerInfo.CopyPlayer(data);
            Debug.Log("info: " + playerInfo._name + "," + playerInfo.colorCode + ", " + playerInfo.costumeIndex);
            SaveSystem.SavePlayer(playerInfo);
            SaveSystem.currentPlayers.Add(data);
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
