using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableArms : MonoBehaviour
{
    public void TurnOn()
    {
        GameManager.Instance.EnableArms(true);
    }

    public void TurnOff()
    {
        GameManager.Instance.EnableArms(false);
    }

    public void EnableInput()
    {
        GameManager.Instance.player.EnableInput();
        GameManager.Instance.EndCutscene();
    }
}
