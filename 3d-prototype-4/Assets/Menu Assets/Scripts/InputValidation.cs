using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InputValidation : MonoBehaviour
{
    public TMP_InputField input;
    public GameObject invalidObj;
    public GameObject submitObj;
    public Menu menuHandler;
    public void CheckInput()
    {
        bool valid = true;
        foreach (string n in menuHandler.players)
        {
            if (n.ToLower() == input.text.ToLower())
            {
                valid = false;
                break;
            }
        }
        // Invalid
        if (input.text.Length < 3 && input.text.Length > 20 || !valid)
        {
            invalidObj.SetActive(true);
            submitObj.SetActive(false);
        }
        else
        {
            invalidObj.SetActive(false);
            submitObj.SetActive(true);
        }
    }

    public void CreateCharacter()
    {
        menuHandler.CreateCharacter(input.text);
    }
}
