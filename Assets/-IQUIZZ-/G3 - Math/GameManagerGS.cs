using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerGS : MonoBehaviour
{
    public GSQUESTIONS[] questions;
    private static List<GSQUESTIONS> unansweredQuestions;
    private GSQUESTIONS currentQuestion;

    [SerializeField] private AudioSource correctSFX;
    [SerializeField] private AudioSource wrongSFX;
    [SerializeField] private AudioSource endGameSFX;

    // Add button references for the three options
    [SerializeField] private Button optionButton1;
    [SerializeField] private Button optionButton2;
    [SerializeField] private Button optionButton3; // New button for third option

    // Add text components for each button
    [SerializeField] private Text optionButton1Text;
    [SerializeField] private Text optionButton2Text;
    [SerializeField] private Text optionButton3Text; // New text component for third option

    [SerializeField] private Image questionImage;
    [SerializeField] private Text factText;
    [SerializeField] private GameObject correctPanel;
    [SerializeField] private GameObject wrongPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private Text correctMessageText;
    [SerializeField] private Text wrongMessageText;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button wrongNextButton;
    [SerializeField] private Button restartButton;

    [SerializeField] private Button homeButtonCorrect;
    [SerializeField] private Button homeButtonWrong;
    [SerializeField] private Button homeButtonEnd;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button homeButtonPause;

    [SerializeField] private Text remainingQuestionsText;

    [SerializeField] private Image wrongPanelLogo;
    [SerializeField] private Sprite defaultWrongLogo;
    [SerializeField] private Sprite timeoutLogo;


    [SerializeField] private float timeLimit = 10f;
    private float timeRemaining;
    private bool isTimerRunning = false;
    private bool isGamePaused = false;

    [SerializeField] private Text timerText;
    [SerializeField] private Slider timerSlider;

    private static int correctAnswersCount = 0;
    [SerializeField] private Text totalCorrectAnswersText;

    void Start()
    {
        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questions.ToList<GSQUESTIONS>();
        }

        SetCurrentQuestion();
        UpdateRemainingQuestionsText();

        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);
        endPanel.SetActive(false);
        pausePanel.SetActive(false);

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

        // Add listeners for each option button
        optionButton1.onClick.AddListener(() => UserSelectOption(0));
        optionButton2.onClick.AddListener(() => UserSelectOption(1));
        optionButton3.onClick.AddListener(() => UserSelectOption(2)); // Listener for third option

        Button pauseButton = GetComponent<Button>();
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
    }

    void Update()
    {
        if (isGamePaused) return;

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

        factText.text = currentQuestion.sentence.Replace("___", "_____");

        // Set each option button's text
        optionButton1Text.text = currentQuestion.options[0];
        optionButton2Text.text = currentQuestion.options[1];
        optionButton3Text.text = currentQuestion.options[2]; // Set text for the third option

        if (currentQuestion.questionImage != null)
        {
            questionImage.sprite = currentQuestion.questionImage;
            questionImage.gameObject.SetActive(true);
        }
        else
        {
            questionImage.gameObject.SetActive(false);
        }

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

    public void UserSelectOption(int optionIndex)
    {
        isTimerRunning = false;

        if (currentQuestion.options[optionIndex] == currentQuestion.correctAnswer)
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

    void ShowWrongPanel(string message, bool isTimeout = false)
    {
        wrongMessageText.text = message;
        wrongPanel.SetActive(true);

        // Set the appropriate logo
        if (isTimeout && timeoutLogo != null)
        {
            wrongPanelLogo.sprite = timeoutLogo;
        }
        else if (defaultWrongLogo != null)
        {
            wrongPanelLogo.sprite = defaultWrongLogo;
        }

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

        if (endGameSFX != null)
        {
            endGameSFX.Play();
        }

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartQuiz);
    }

    public void RestartQuiz()
    {
        unansweredQuestions = questions.ToList<GSQUESTIONS>();
        correctAnswersCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void TimeOut()
    {
        ShowWrongPanel("Time's up! Better luck next time!", true);
    }

    public void GoToMainMenu()
    {
        unansweredQuestions = null;
        correctAnswersCount = 0;
        SceneManager.LoadScene("MainMenuTwo");
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
}
