using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    //singleton pattern
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //   DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    //for test
    public SceneController sceneController;
    GameController gameController;
    Pooler pooler;
    int score;

    [Header("Timer variables")]
    float timer;
    bool canCount = false;
    int minutes;
    int seconds;
    string timeStr;

    [Header("UI variables")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public CanvasGroup GameplayPanel;
    public CanvasGroup FinishPanel;
    public TextMeshProUGUI FinishScore;

    [Header("Menu variables")]
    public GameObject StartArcadeModeButton;
    //public GameObject StartSurvivorModeButton;
    public GameObject MainMenuButton;

    private void Start()
    {
        gameController = GameController.instance;
        pooler = Pooler.instance;
        GameplayPanel.alpha = 0;
        FinishPanel.alpha = 0;
      //  StartArcadeMode();
    }

    private void Update()
    {
        if (timer > 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            minutes = Mathf.FloorToInt(timer / 60f);
            seconds = Mathf.FloorToInt(timer - minutes * 60);
            timeStr = string.Format("{0:0} : {1:00}", minutes, seconds);
            timeText.text = timeStr;
        }
        else if (timer <= 0.0f && canCount)
        {
            Finish();
        }
    }

    //start our game funciton,
    public void StartArcadeMode()
    {
        StartArcadeModeButton.SetActive(false);
        MainMenuButton.SetActive(false);
        GameplayPanel.alpha = 1;
        FinishPanel.alpha = 0;
        score = 0;
        timer = 60f;
        canCount = true;
        timeText.text = " 1 : 00";
        scoreText.text = "0";
        pooler.StartGameInPool(0);
    }

    public void IncreaseScore(int count)
    {
        score += count;
        scoreText.text = score.ToString();
    }
    public void Finish()
    {
        pooler.ResetPool();
        GameplayPanel.alpha = 0;
        FinishPanel.alpha = 1;
        FinishScore.text = score.ToString();

        StartArcadeModeButton.SetActive(true);
        MainMenuButton.SetActive(true);

        canCount = false;
        timeText.text = "0 : 00";
    }

    //for now, there isn't a survivor mod
    /*    
    public void StartSurviorMode()
    {
            score = 0;
            timer = 0f;
            canCount = true;
            timeText.text = " 0 : 00";
            scoreText.text = "0";
    }
    */

    public void StartMainMenu()
    {
        pooler.ResetPool();
        canCount = false;
        sceneController.LoadThisScene("MainMenu");
    }

    //for close all buttons
    public void BeforeSelectButton()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
    }

    //function for multiple mod but now it's just arcade mode
    public void SelectButton(string _fruitParentButton)
    {
        switch (_fruitParentButton)
        {
            case "Arcade":
                StartArcadeMode();
                break;
            /*             case "Survivor":
                            StartSurviorMode();
                            break; */
            case "MainMenu":
                StartMainMenu();
                break;
            default:
                Debug.Log("Please Check your Button's name!");
                break;
        }
    }
}


