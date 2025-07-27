using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public GameObject rightArm;
    public GameObject leftArm;
    public GameObject playerHead;
    public GameState gameState;
    public bool hasArms;
    public Animator camAnimator;
    public UnityEvent onSceneStart;
    public List<Cutscene> worldHudCutscenes;
    [SerializeField] private bool debugMode;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ChangeGameState(gameState);
    }

    public

    void Update()
    {
        switch (gameState)
        {
            case GameState.Start:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeGameState(GameState.SpawnPlayer);
                }
                break;

            case GameState.Cinematic:

                break;

            case GameState.Load:
                break;

            case GameState.Active:
                break;
        }
    }

    public void ActivateGameState()
    {
        ChangeGameState(GameState.Active);
    }
    public void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;

        switch (gameState)
        {
            case GameState.Start:
                MenuManager.Instance.StartGame();
                camAnimator.enabled = false;
                EnableArms(false);
                player.DisableInput();
                break;
            case GameState.SpawnPlayer:
                if (debugMode)
                {
                    camAnimator.enabled = true;
                    EnableArms(true);
                    HUDController.Instance.OpenScene();
                    WorldController.Instance.Reset();

                    Invoke(nameof(ActivateGameState), .25f);
                }
                else
                {
                    player.DisableInput();
                    MenuManager.Instance.OpenNewScene(0);
                }

                break;
            case GameState.Active:
                player.EnableInput();
                break;
            case GameState.Load:
                break;

            case GameState.End:
                break;
        }
    }

    public void PlayCutscene(int index)
    {
        camAnimator.SetBool("IsCinematic", true);
        worldHudCutscenes[index].StartCutscene();
    }
    public void EndCutscene()
    {
        if (hasArms) camAnimator.SetBool("IsCinematic", false);
    }

    public void EndScene()
    {
        HUDController.Instance.TurnOn(false);
        HUDController.Instance.canvasAnimator.SetBool("IsOpen", false);
        player.DisableInput();
        camAnimator.SetBool("IsCinematic", true);
    }

    public void EnableArms(bool isOn)
    {
        rightArm.SetActive(isOn);
        leftArm.SetActive(isOn);
        hasArms = isOn;
        if (isOn)
        {
            player.CanFight(true);
            player.DisableInput();
            camAnimator.Play("ArmUp");
            camAnimator.enabled = true;
            HUDController.Instance.TurnOn(true);
        }
    }

    public void TransitionToNewWorld(int index)
    {
        player.DisableInput();
        MenuManager.Instance.OpenNewScene(index);
    }

    public Vector3 RandomDirection(float minAngle, float maxAngle)
    {
        // Convert angles to radians
        float minRad = minAngle * Mathf.Deg2Rad;
        float maxRad = maxAngle * Mathf.Deg2Rad;

        // Random horizontal rotation around the Y-axis
        float horizontalAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Random vertical angle between min and max
        float verticalAngle = Random.Range(minRad, maxRad);

        // Spherical coordinates to Cartesian conversion
        float x = Mathf.Cos(verticalAngle) * Mathf.Cos(horizontalAngle);
        float y = Mathf.Sin(verticalAngle); // vertical lift
        float z = Mathf.Cos(verticalAngle) * Mathf.Sin(horizontalAngle);

        return new Vector3(x, y, z).normalized;
    }

    public Vector3 ApplyTorque(Vector3 direction, float torqueStrength)
    {
        // Create a perpendicular axis for torque based on the input direction
        Vector3 torqueAxis = Vector3.Cross(direction.normalized, Vector3.up);

        // If the direction is vertical, use a default axis (to avoid zero vector)
        if (torqueAxis == Vector3.zero)
            torqueAxis = Vector3.right;

        return torqueAxis.normalized * torqueStrength;
    }

}
public enum GameState{
    Start,
    SpawnPlayer,
    Active,
    Cinematic,
    Load,
    End,
}