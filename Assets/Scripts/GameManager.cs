using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.Build;

public class GameManager : MonoBehaviour
{
    //Keybinds
    [SerializeField] private InputActionReference pause;

    //object References
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private TextMeshProUGUI endText;

    //variables
    public Image healthBar;
    public GameObject playerCombat;
    private bool paused, ended, controls;
    [SerializeField] private int numEnemies;
    private int deadCount;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        paused = false;
        ended = false;
        deadCount = 0;

        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
        controlsScreen.SetActive(false);

        controls = false;
    }

    // Update is called once per frame
    void Update()
    {
        showHealth();
    }

    //input start
    private void OnEnable()
    {
        pause.action.performed += pauseGame;
    }

    //input end
    private void OnDisable()
    {
        pause.action.performed += pauseGame;
    }

    //function to take InputAction to pause
    private void pauseGame(InputAction.CallbackContext context)
    {
        if (controls)
        {
            ControlMenu();
        }
        else
        {
            Pause();
        } 
    }

    //function that pauses the game
    public void Pause()
    {
        if (!ended)
        {
            if (!paused)
            {
                paused = !paused;
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
            }
            else
            {
                paused = !paused;
                Time.timeScale = 1;
                pauseScreen.SetActive(false);
            }
        }
        
    }

    //function that restarts level
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    //function that exits game
    public void Exit()
    {
        Application.Quit();
    }

    public void endGame(string endMessage)
    {
        ended = true;
        Time.timeScale = 0;
        endText.text = endMessage;
        endScreen.SetActive(true);
    }

    public void enemyKilled()
    {
        deadCount++;

        if(deadCount >= numEnemies)
        {
            endGame("You Win!");
        }
    }

    //function to display HealthBar
    private void showHealth()
    {
        healthBar.fillAmount = getHealth() / 100f;
    }

    //Get health from combat component
    private float getHealth()
    {
        return playerCombat.GetComponent<PlayerCombat>().getHealth();
    }

    //function that returns the pause state of the game controller.
    public bool getPaused()
    {
        return paused;
    }

    //function that toggles the control menu during pause.
    public void ControlMenu()
    {
        controls = !controls;
        controlsScreen.SetActive(controls);
        pauseScreen.SetActive(!controls);
    }
}
