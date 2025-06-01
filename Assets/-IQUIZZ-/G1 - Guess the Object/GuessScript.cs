using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuessScript : MonoBehaviour
{
    public GuessQuestions[] questions;
    private static List<GuessQuestions> unansweredQuestions;
    private GuessQuestions currentQuestion;

    [SerializeField] private InputField nameInputField; // Input field for user's name
    [SerializeField] private Text leaderboardText; // Text component to display leaderboard entries
    [SerializeField] private Button submitButton; // Submit button for saving the score

    private const string LeaderboardKey = "Leaderboard";



    [SerializeField] private Image questionImageComponent; // Reference for the question image


    [SerializeField]
    private AudioSource correctSFX;
    [SerializeField]
    private AudioSource wrongSFX;
    [SerializeField]
    private AudioSource endGameSFX;

    // Add these two button references
    [SerializeField] private Button optionButton1; // Button for the first option
    [SerializeField] private Button optionButton2; // Button for the second option
    [SerializeField] private Button optionButton3; // Button for the second option



    // Add these references for button texts
    [SerializeField] private Text optionButton1Text; // Text component for the first option button
    [SerializeField] private Text optionButton2Text; // Text component for the second option button
    [SerializeField] private Text optionButton3Text; // Text component for the second option button


    [SerializeField] private Text questionTextComponent; // Text component for the question text


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

    [SerializeField] private Image wrongPanelLogo;
    [SerializeField] private Sprite defaultWrongLogo;
    [SerializeField] private Sprite timeoutLogo;


    private static int correctAnswersCount = 0;
    [SerializeField]
    private Text totalCorrectAnswersText;

    void Start()
    {
        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            unansweredQuestions = questions.ToList<GuessQuestions>();
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
        optionButton1.onClick.AddListener(() => UserSelectOption(0)); // For the first option
        optionButton2.onClick.AddListener(() => UserSelectOption(1)); // For the second option
        optionButton3.onClick.AddListener(() => UserSelectOption(2)); // For the second option


        // Set up Pause Button (make sure you assign this button in Unity Editor)
        Button pauseButton = GetComponent<Button>(); // Add reference to your Pause Button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }

        submitButton.onClick.AddListener(OnSubmit);

        // Initialize leaderboard display
        UpdateLeaderboard();

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

        // Display the leaderboard
        UpdateLeaderboard();

    }

    void OnSubmit()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous"; // Default name if none is entered
        }

        // Save the player's name and score
        SaveScore(playerName, correctAnswersCount);

        // Update the leaderboard
        UpdateLeaderboard();

        // Clear the name input field
        nameInputField.text = "";

        // Optionally disable the Submit button after saving
        submitButton.interactable = false;
    }

    void SaveScore(string playerName, int score)
    {
        // Load existing leaderboard data
        List<LeaderboardEntry> leaderboard = LoadLeaderboard();

        // Add the new score
        leaderboard.Add(new LeaderboardEntry { Name = playerName, Score = score });

        // Sort the leaderboard by score in descending order
        leaderboard = leaderboard.OrderByDescending(entry => entry.Score).Take(10).ToList();

        // Save the updated leaderboard
        string json = JsonUtility.ToJson(new LeaderboardWrapper { Entries = leaderboard });
        PlayerPrefs.SetString(LeaderboardKey, json);
        PlayerPrefs.Save();
    }

    List<LeaderboardEntry> LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LeaderboardKey))
        {
            string json = PlayerPrefs.GetString(LeaderboardKey);
            return JsonUtility.FromJson<LeaderboardWrapper>(json).Entries;
        }
        return new List<LeaderboardEntry>();
    }

void UpdateLeaderboard()
{
    // Load leaderboard entries
    List<LeaderboardEntry> leaderboard = LoadLeaderboard();

    // Initialize leaderboard text with headers and a separator
    leaderboardText.text = "Rank   Name         Score\n\n";
   

    if (leaderboard.Count == 0)
    {
        // Display message if no scores are present
        leaderboardText.text += "No entries yet.";
        return;
    }

    // Define column widths
    int rankWidth = 5;   // Rank column width
    int nameWidth = 10;  // Name column width
    int scoreWidth = 6;  // Score column width

    // Loop through the leaderboard and format each entry
    for (int i = 0; i < leaderboard.Count; i++)
    {
        LeaderboardEntry entry = leaderboard[i];

        string rank = (i + 1).ToString().PadRight(rankWidth); // Rank
        string name = TruncateString(entry.Name, nameWidth).PadRight(nameWidth); // Name truncated
        string score = entry.Score.ToString().PadLeft(scoreWidth); // Right-aligned Score

        leaderboardText.text += $"{rank}{name}{score}\n";
    }
}

// Helper function to truncate strings to a specified length
private string TruncateString(string value, int maxLength)
{
    if (string.IsNullOrEmpty(value)) return string.Empty;
    return value.Length <= maxLength ? value : value.Substring(0, maxLength);
}




    [System.Serializable]
    public class LeaderboardEntry
    {
        public string Name;
        public int Score;
    }

    [System.Serializable]
    public class LeaderboardWrapper
    {
        public List<LeaderboardEntry> Entries;
    }

    void SetCurrentQuestion()
    {
        int randomQuestionIndex = Random.Range(0, unansweredQuestions.Count);
        currentQuestion = unansweredQuestions[randomQuestionIndex];

        // Display the question image
        questionImageComponent.sprite = currentQuestion.questionImage;

        // Set the question text
        questionTextComponent.text = currentQuestion.questionText;

        // Set the images and texts for the options
        optionButton1.GetComponentInChildren<Image>().sprite = currentQuestion.optionImages[0];
        optionButton1.GetComponentInChildren<Text>().text = currentQuestion.optionTexts[0];

        optionButton2.GetComponentInChildren<Image>().sprite = currentQuestion.optionImages[1];
        optionButton2.GetComponentInChildren<Text>().text = currentQuestion.optionTexts[1];

        optionButton3.GetComponentInChildren<Image>().sprite = currentQuestion.optionImages[2];
        optionButton3.GetComponentInChildren<Text>().text = currentQuestion.optionTexts[2];


        // Reset the timer and start it
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

        // Compare selected image with the correct answer
        if (currentQuestion.optionImages[optionIndex] == currentQuestion.correctAnswer)
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
        unansweredQuestions = questions.ToList<GuessQuestions>();
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
        correctAnswersCount = 0; // Reset the correct answers count
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