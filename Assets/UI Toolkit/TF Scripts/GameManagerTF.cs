using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerTF : MonoBehaviour
{
    public Question[] questions;
    private static List<Question> unansweredQuestions;
    private Question currentQuestion;

    [SerializeField]
    private AudioSource correctSFX;
    [SerializeField]
    private AudioSource wrongSFX;
    [SerializeField]
    private AudioSource endGameSFX;

    [SerializeField]
    private Text factText;

    [SerializeField]
    private GameObject correctPanel;
    [SerializeField]
    private GameObject wrongPanel;
    [SerializeField]
    private GameObject endPanel;
    [SerializeField]
    private GameObject pausePanel;    // Pause panel for pausing the game

    [SerializeField]
    private Text correctMessageText;
    [SerializeField]
    private Text wrongMessageText;

    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button wrongNextButton;
    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button homeButtonCorrect;
    [SerializeField]
    private Button homeButtonWrong;
    [SerializeField]
    private Button homeButtonEnd;

    [SerializeField]
    private Button resumeButton;     // Resume button on the pause panel
    [SerializeField]
    private Button pauseRestartButton; // Restart button on the pause panel
    [SerializeField]
    private Button homeButtonPause;    // Home button on the pause panel

    [SerializeField]
    private Text remainingQuestionsText;

    [SerializeField] private float timeLimit = 10f;
    private float timeRemaining;
    private bool isTimerRunning = false;
    private bool isGamePaused = false; // Track whether the game is paused

    [SerializeField] private Text timerText;
    [SerializeField] private Slider timerSlider;

    private static int correctAnswersCount = 0;
    [SerializeField]
    private Text totalCorrectAnswersText;

    void Start()
    {
        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questions.ToList<Question>();
        }

        SetCurrentQuestion();
        UpdateRemainingQuestionsText();

        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);
        endPanel.SetActive(false);
        pausePanel.SetActive(false);    // Hide the pause panel initially

        if (timerSlider != null)
        {
            timerSlider.maxValue = timeLimit;
            timerSlider.value = timeLimit;
        }
        timerText.text = timeLimit.ToString("F2");

        homeButtonCorrect.onClick.AddListener(GoToMainMenu);
        homeButtonWrong.onClick.AddListener(GoToMainMenu);
        homeButtonEnd.onClick.AddListener(GoToMainMenu);

        resumeButton.onClick.AddListener(ResumeGame);
        pauseRestartButton.onClick.AddListener(RestartQuiz);
        homeButtonPause.onClick.AddListener(GoToMainMenu);

        // Set up Pause Button (make sure you assign this button in Unity Editor)
        Button pauseButton = GetComponent<Button>(); // Add reference to your Pause Button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
    }

    void Update()
    {
        if (isGamePaused) return; // Skip the game update if paused

        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();

            if (timerSlider != null)
            {
                timerSlider.value = timeRemaining;
            }

            if (timeRemaining <= 0)
            {
                isTimerRunning = false;
                TimeOut();
            }
        }
    }

    void SetCurrentQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomQuestionIndex];

        factText.text = currentQuestion.fact;

        timeRemaining = timeLimit;
        isTimerRunning = true;
    }

    void TransitionToNextQuestion()
    {
        unansweredQuestions.Remove(currentQuestion);

        if (unansweredQuestions.Count > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            ShowEndPanel();
        }

        UpdateRemainingQuestionsText();
        isTimerRunning = false;
    }

    void UpdateRemainingQuestionsText()
    {
        int answeredQuestions = questions.Length - unansweredQuestions.Count;
        int totalQuestions = questions.Length;
        remainingQuestionsText.text = $"{answeredQuestions}/{totalQuestions}";
    }

    public void UserSelectTrue()
    {
        isTimerRunning = false;

        if (currentQuestion.isTrue)
        {
            correctAnswersCount++;
            ShowCorrectPanel("Good Job!");
        }
        else
        {
            ShowWrongPanel("Better luck next time!");
        }
    }

    public void UserSelectFalse()
    {
        isTimerRunning = false;

        if (!currentQuestion.isTrue)
        {
            correctAnswersCount++;
            ShowCorrectPanel("Good Job!");
        }
        else
        {
            ShowWrongPanel("Better luck next time!");
        }
    }

    void ShowCorrectPanel(string message)
    {
        correctMessageText.text = message;
        correctPanel.SetActive(true);

        // Play correct answer SFX
        if (correctSFX != null)
        {
            correctSFX.Play();
        }

        if (unansweredQuestions.Count > 1)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(TransitionToNextQuestion);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            ShowEndPanel();
        }
    }

    void ShowWrongPanel(string message)
    {
        wrongMessageText.text = message;
        wrongPanel.SetActive(true);

        // Play wrong answer SFX
        if (wrongSFX != null)
        {
            wrongSFX.Play();
        }

        if (unansweredQuestions.Count > 1)
        {
            wrongNextButton.onClick.RemoveAllListeners();
            wrongNextButton.onClick.AddListener(TransitionToNextQuestion);
        }
        else
        {
            wrongNextButton.gameObject.SetActive(false);
            ShowEndPanel();
        }
    }

    void ShowEndPanel()
    {
        endPanel.SetActive(true);
        totalCorrectAnswersText.text = "Correct Answers: " + correctAnswersCount + "/" + questions.Length;

        // Play end game SFX
        if (endGameSFX != null)
        {
            endGameSFX.Play();
        }

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartQuiz);
    }


    public void RestartQuiz()
    {
        unansweredQuestions = questions.ToList<Question>();
        correctAnswersCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void TimeOut()
    {
        ShowWrongPanel("Time's up! Better luck next time!");
    }

    public void GoToMainMenu()
    {
        unansweredQuestions = null;
        SceneManager.LoadScene("MainMenuTwo");
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0; // Pause the game time
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1; // Resume the game time
        pausePanel.SetActive(false);
    }
}
