using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;
    public bool debugMode = true;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!debugMode)
            SceneWorldManager.Instance.TransferToWorld(0);
    }
}

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
    The Test Room
    Magatsu Inaba
    FPP Lab
    Andover High Lunch Room
    Vineyard House
    Apartment
    Area 88
    Squid Game Start Room
    The Skeld
    My Desk
    Ikea
    Golisano Atrium
*/
