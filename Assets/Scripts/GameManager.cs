using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// GameManager class responsible for managing game state and logic.
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }
    
    public GameState currentState;//stores the current state of the game
    public GameState previousState;//stores the previous state of the game before pause

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas; //used to draw the floating text
    public float textFontSize = 20; //adjustable font size
    public TMP_FontAsset textFont;
    public Camera referenceCamera; //allows the ability to know where on the canvas the text should appear

    [Header("Screens")]
    public GameObject pauseScreen;//game object to store the pause menu
    public GameObject resultsScreen;//game object to store the game over menu
    public GameObject levelUpScreen;//game object to store the level up menu
    int stackedLevelUps = 0; //if player levels up multiple times at once

    [Header("Current Stats Display")]
    //current stat display
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisply;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    [Header("Result Screen Display")]
    public Image chosenCharacterImage;
    public TextMeshProUGUI chosenCharacterName;
    public TextMeshProUGUI levelReachedDisplay;
    public TextMeshProUGUI timeSurvivedDisplay;

    [Header("Stopwatch")]
    public float timeLimit; //holds time limit in seconds
    float stopwatchTime; //current elapsed time since started
    public TextMeshProUGUI stopwatchDisplay;

    public GameObject playerObject; //reference player game object

    //maintains compatibility with other scripts
    public bool isGameOver
    {
        get
        {
            return currentState == GameState.GameOver;
        }
    }

    public bool choosingUpgrade
    {
        get
        {
            return currentState == GameState.LevelUp;
        }
    }

    void Awake()
    {
        //check to see if there is another singleton of this kind
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + " DELETED");
            Destroy(gameObject);
        }

        DisableScreens();
    }

    void Update()
    {
        //Defining the behavior for each game state
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
            case GameState.LevelUp:
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        /*begins generating floating text and is responsible for animation after text is spawned*/
        GameObject textObject = new GameObject("Damage Floating Text");
        RectTransform rect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObject.AddComponent<TextMeshProUGUI>();

        //sets text settings
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;

        if (textFont)
        {
            tmPro.font = textFont;
        }

        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObject, duration);//destrys the text object after the duration
        textObject.transform.SetParent(instance.damageTextCanvas.transform); //parents the generated text to the canvas
        textObject.transform.SetSiblingIndex(0); //sets objects as the first object

        //pan text upwards and fade away
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPostion = target.position;

        while (t < duration) 
        {
            //break loop if rect is missing for some reason
            if (!rect)
            {
                break;
            }

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration); //fade the text

            if (target)
            {
                //pan text upward
                yOffset += speed * Time.deltaTime;
                rect.position = referenceCamera.WorldToScreenPoint(lastKnownPostion + new Vector3(0, yOffset));
            }
            else
            {
                //if target is dead
                rect.position += new Vector3(0, speed * Time.deltaTime, 0);
            }

            //wait for a frame and update the time
            yield return w;
            t += Time.deltaTime;
        }
    }

    //method to make floating text appear
    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        //ends function if there is no canvas
        if (!instance.damageTextCanvas)
        {
            Debug.Log("No Canvas Found");
            return; 
        }
        //finds a relevant camera that is used to convert the world position to screen position
        if (!instance.referenceCamera)
        {
            instance.referenceCamera = Camera.main;  
        }

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }

    //allows control over all state changes
    public void ChangeState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    public void PauseGame()
    {
        //checks if the game is not already paused
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;//stop the game
            pauseScreen.SetActive(true);//activates pause screen
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;//resumes the game
            pauseScreen.SetActive(false);//deactivates pause screen
        }
    }

    //checks for input to pause or resume game
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    //reference to deactivate all game screens
    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;

        //sets game over state variables
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f; //pause game
        DisplayResults();
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime; //increments the stopwatch by the amount of the time passed since the last frame

        UpdateStopwatchDisplay();

        //game ends if the stopwatch reaches or exceeds the set time limit
        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }
    
    //updates displayed stopwatch text that is on screen
    void UpdateStopwatchDisplay()
    {
        //rounds down and converts stopwatchTime to the nearest integer to calculate the number of minutes and seconds that have passed
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        //update and display stopwatch text
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    //used to enter the level up screen
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);

        //record level up screen if already active
        if (levelUpScreen.activeSelf)
        {
            stackedLevelUps++;
        }
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f; //pause game
            playerObject.SendMessage("RemoveAndApplyUpgrades");
        }
    }

    //used to leave the level up screen
    public void EndLevelUp()
    {
        Time.timeScale = 1f; //Resumes time when leaving menu
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);

        if (stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp();
        }
    }
}
