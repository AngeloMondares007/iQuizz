using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    [SerializeField] private GameObject gameComplete; // Reference to the game complete panel
    [SerializeField] private GameObject timesUpPanel; // Panel to show when time's up
    [SerializeField] private GameObject correctAnswerPanel; // New panel for correct answers
    [SerializeField] private float timeUpDisplayDuration = 2f; // Time to show the Time's Up panel
    [SerializeField] private QuizDataScriptable questionDataScriptable;
    [SerializeField] private Image questionImage;
    [SerializeField] private WordData[] answerWordList = new WordData[4]; // Limit to 4
    [SerializeField] private WordData[] optionsWordList = new WordData[6]; // Limit to 6
    [SerializeField] private Text gameOverCorrectAnswersText;

    [SerializeField] private Text timerText;
    [SerializeField] private Text questionCounterText;
    [SerializeField] private float timePerQuestion = 30f;

    [SerializeField] private GameObject pausePanel;

    [SerializeField] private AudioClip wrongAnswerSFX;
    [SerializeField] private AudioClip correctAnswerSFX;
    [SerializeField] private AudioClip timesUpSFX;
    [SerializeField] private AudioClip gameCompleteSFX;

    private AudioSource audioSource;
    private GameStatus gameStatus = GameStatus.Playing;
    private char[] wordsArray = new char[6]; // Update to match `optionsWordList`

    private int totalQuestions;
    private int totalCorrectAnswers = 0;

    private List<int> selectedWordsIndex;
    private int currentAnswerIndex = 0, currentQuestionIndex = 0;
    private bool correctAnswer = true;
    private string answerWord;
    private List<QuestionData> shuffledQuestions;
    private float timeRemaining;

    private Coroutine timerCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        selectedWordsIndex = new List<int>();
        shuffledQuestions = questionDataScriptable.questions.OrderBy(q => UnityEngine.Random.value).ToList();
        totalQuestions = shuffledQuestions.Count;
        SetQuestion();
    }

    void SetQuestion()
    {
        gameStatus = GameStatus.Playing;
        timeRemaining = timePerQuestion;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(Timer());

        answerWord = shuffledQuestions[currentQuestionIndex].answer;
        questionImage.sprite = shuffledQuestions[currentQuestionIndex].questionImage;

        ResetQuestion();

        selectedWordsIndex.Clear();
        Array.Clear(wordsArray, 0, wordsArray.Length);

        for (int i = 0; i < Mathf.Min(answerWord.Length, answerWordList.Length); i++)
        {
            wordsArray[i] = char.ToUpper(answerWord[i]);
        }

        for (int j = answerWord.Length; j < wordsArray.Length; j++)
        {
            wordsArray[j] = (char)UnityEngine.Random.Range(65, 90);
        }

        wordsArray = ShuffleList.ShuffleListItems<char>(wordsArray.ToList()).ToArray();

        for (int k = 0; k < optionsWordList.Length; k++)
        {
            optionsWordList[k].SetWord(wordsArray[k]);
        }

        questionCounterText.text = $"{currentQuestionIndex + 1}/{shuffledQuestions.Count}";
    }

    public void ResetQuestion()
    {
        for (int i = 0; i < answerWordList.Length; i++)
        {
            if (i < answerWord.Length)
            {
                answerWordList[i].gameObject.SetActive(true);
                answerWordList[i].SetWord('_');
            }
            else
            {
                answerWordList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < optionsWordList.Length; i++)
        {
            optionsWordList[i].gameObject.SetActive(true);
        }

        currentAnswerIndex = 0;
    }


    public void SelectedOption(WordData value)
    {
        if (gameStatus == GameStatus.Next || currentAnswerIndex >= answerWord.Length) return;

        selectedWordsIndex.Add(value.transform.GetSiblingIndex());
        value.gameObject.SetActive(false);
        answerWordList[currentAnswerIndex].SetWord(value.wordValue);

        currentAnswerIndex++;

        if (currentAnswerIndex == answerWord.Length)
        {
            correctAnswer = true;
            for (int i = 0; i < answerWord.Length; i++)
            {
                if (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordList[i].wordValue))
                {
                    correctAnswer = false;
                    break;
                }
            }

            if (correctAnswer)
            {
                Debug.Log("Correct Answer");
                totalCorrectAnswers++; // Increment the correct answers count
                gameStatus = GameStatus.Next;
                audioSource.PlayOneShot(correctAnswerSFX);
                correctAnswerPanel.SetActive(true); // Show correct answer panel
                StartCoroutine(HideCorrectAnswerPanel()); // Start the coroutine to hide it after 2 seconds
                StartCoroutine(ProceedToNextQuestionAfterDelay(2f)); // Wait before proceeding to the next question
            }
            else
            {
                Debug.Log("Incorrect Answer");
                audioSource.PlayOneShot(wrongAnswerSFX);
                foreach (var wordData in answerWordList)
                {
                    wordData.SetWrongAnswerStyle();
                }
                Invoke("ResetQuestionStyle", 1f);
            }
        }
    }


    // New Coroutine for proceeding to the next question after a delay
    private IEnumerator ProceedToNextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for 2 seconds before proceeding
        currentQuestionIndex++;

        if (currentQuestionIndex < shuffledQuestions.Count)
        {
            SetQuestion(); // Set the next question
        }
        else
        {
            Debug.Log("Game Complete");
            StopTimer();
            // Play the game complete sound effect
            audioSource.PlayOneShot(gameCompleteSFX);

            gameComplete.SetActive(true);

            // Display the total correct answers in the game over panel
            gameOverCorrectAnswersText.text = $"{totalCorrectAnswers}/{totalQuestions}";
        }
    }

    public void ResetLastWord()
    {
        if (selectedWordsIndex.Count > 0)
        {
            int index = selectedWordsIndex[selectedWordsIndex.Count - 1];
            optionsWordList[index].gameObject.SetActive(true);
            selectedWordsIndex.RemoveAt(selectedWordsIndex.Count - 1);

            currentAnswerIndex--;
            answerWordList[currentAnswerIndex].SetWord('_');

            // Reset the style of the last selected word
            optionsWordList[index].ResetStyle(); // Reset the style for the word
        }
    }

    // Resets the styles for the options and answers without resetting the timer
    public void ResetQuestionStyle()
    {
        foreach (var word in answerWordList)
        {
            word.ResetStyle(); // Reset each answer word style
        }

        foreach (var option in optionsWordList)
        {
            option.ResetStyle(); // Reset each option word style
        }
    }

    IEnumerator HideCorrectAnswerPanel()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        correctAnswerPanel.SetActive(false); // Hide the panel
    }

    IEnumerator Timer()
    {
        while (timeRemaining > 0)
        {
            timerText.text = $"{Mathf.CeilToInt(timeRemaining)}";
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        // Time's up, treat as incorrect answer
        if (timeRemaining <= 0)
        {
            Debug.Log("Time's Up!");
            gameStatus = GameStatus.Next;

            // Play the time's up sound effect
            audioSource.PlayOneShot(timesUpSFX);

            // Show the Time's Up panel
            timesUpPanel.SetActive(true);

            // Wait for a few seconds before proceeding
            yield return new WaitForSeconds(timeUpDisplayDuration);

            // Hide the Time's Up panel
            timesUpPanel.SetActive(false);

            currentQuestionIndex++;

            if (currentQuestionIndex < shuffledQuestions.Count)
            {
                Invoke("SetQuestion", 0.0f);
            }
            else
            {
                Debug.Log("Game Complete");
                StopTimer(); // Stop the timer when the game is complete
                gameComplete.SetActive(true);

                // Display the total correct answers in the format "X/Y"
                gameOverCorrectAnswersText.text = $"{totalCorrectAnswers}/{totalQuestions}";
            }
        }
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    public void PauseGame()
    {
        if (gameStatus == GameStatus.Playing)
        {
            gameStatus = GameStatus.Next;
            StopTimer(); // Stop the timer coroutine
            pausePanel.SetActive(true);
            Debug.Log("Game Paused");
        }
    }

    public void ResumeGame()
    {
        if (gameStatus == GameStatus.Next)
        {
            gameStatus = GameStatus.Playing;
            timerCoroutine = StartCoroutine(Timer()); // Restart the timer
            pausePanel.SetActive(false);
            Debug.Log("Game Resumed");
        }
    }

    public void ResetGame()
    {
        currentQuestionIndex = 0;
        selectedWordsIndex.Clear();
        totalCorrectAnswers = 0; // Reset total correct answers
        gameComplete.SetActive(false);
        timesUpPanel.SetActive(false);
        correctAnswerPanel.SetActive(false); // Ensure the correct answer panel is hidden
        ResetQuestionStyle(); // Reset the styles for all words
        SetQuestion();
        pausePanel.SetActive(false);
        Debug.Log("Game Reset");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuTwo");
        Debug.Log("Going to Main Menu");
    }
}

[System.Serializable]
public class QuestionData
{
    public Sprite questionImage;
    public string answer;
}

public enum GameStatus
{
    Next,
    Playing
}
