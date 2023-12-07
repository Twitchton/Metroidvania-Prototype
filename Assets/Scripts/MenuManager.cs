using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //object references
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject creditsMenu;

    //variables
    bool controls;
    bool credits;

    // Start is called before the first frame update
    void Start()
    {
        controls = false;
        credits = false;

        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //loads the game scene
    public void StartGame()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    //displays controls menu
    public void Controls()
    {
        controls = !controls;
        controlsMenu.SetActive(controls);
        mainMenu.SetActive(!controls);
    }

    //displays credits menu
    public void Credits()
    {
        credits = !credits;
        creditsMenu.SetActive(credits);
        mainMenu?.SetActive(!credits);
    }

    //exits game
    public void Exit()
    {
        Application.Quit();
    }
}
