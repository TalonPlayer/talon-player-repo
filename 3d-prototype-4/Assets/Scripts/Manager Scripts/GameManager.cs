using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;
    public bool debugMode = true;
    public Toggle tutorialToggle; // If the current world has a tutorial
    void Awake()
    {
        Instance = this;

        // Saves if the tutorial is turned off or on during play
        bool tutorialOn = PlayerPrefs.GetInt("ToggleTutorial", 0) == 0;
        tutorialToggle.isOn = tutorialOn;
        tutorialToggle.onValueChanged.AddListener(SetTutorial);
    }

    void Start()
    {
        // If not debugging, send the player to the first world
        if (!debugMode)
            SceneWorldManager.Instance.TransferToWorld(0);
        else // Debug mode is on
        {
            HudManager.Instance.ToggleBlackScreen(false);
        }
    }
    
    /// <summary>
    /// Toggles the tutorial so that its the same throughout play
    /// </summary>
    /// <param name="isOn"></param>
    public void SetTutorial(bool isOn)
    {
        PlayerPrefs.SetInt("ToggleTutorial", isOn ? 0 : 1);
        PlayerPrefs.Save();
    }
}

// Not Used
public enum GameState
{
    Active,
    Roam,
    Pause,
    Transfer
}

// TODO:
/*
    Add Menu
    Add Pause Mechanic
    Add More weapons
    Add More special drops
    Add Special effects for zombie death
    Add sounds for player and zombies <-
    Add Tesla Towers

    Special Drops:
    Obama Pic
    Bird Pic
    Skull Rain - 10 Skulls drop from the sky
    Ammo Drop - Gives unlimited ammo for current ammo for 30 seconds

    Ankle Biters


    Map Ideas
    Island
    Subway
    Dojo
    Bridge
    Dungeon
    Cube World
    Village
    Skyscrapers
    Graveyard
    Hospital
    Cyberpunk Alleyway
    Factory
    Cliffside
    
    
    Easter Egg Maps
    The Test Room <- Done
    Magatsu Inaba
    FPP Lab
    Andover High Lunch Room
    Vineyard House
    Apartment <- Done
    Area 88
    Squid Game Start Room
    The Skeld
    My Desk
    Ikea
    Golisano Atrium
*/
