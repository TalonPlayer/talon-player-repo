using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Animator menuAnimator;
    public SceneField universalGameplay;
    void Start()
    {

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

    public IEnumerator OpenGame()
    {
        yield return new WaitForSeconds(7f);

        SceneManager.LoadScene(universalGameplay);
    }
}
