using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // For handling UI events

public class WordData : MonoBehaviour
{
    [SerializeField] private Text wordText;
    [SerializeField] private Color wrongAnswerColor = Color.red; // Color for wrong answers
    [SerializeField] private Color defaultColor = Color.white; // Default color for correct answers
    [SerializeField] private Outline outlineComponent; // Reference to Outline component

    [HideInInspector]
    public char wordValue;

    private Button buttonComponent;

    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
        if (buttonComponent)
        {
            buttonComponent.onClick.AddListener(() => WordSelected());
        }

        // Ensure the outline component is present; if not, add it dynamically.
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
        {
            outlineComponent = gameObject.AddComponent<Outline>();
        }
        outlineComponent.enabled = false; // Disable outline by default
    }

    public void SetWord(char value)
    {
        wordText.text = value.ToString();
        wordValue = value;
    }

    private void WordSelected()
    {
        QuizManager.instance.SelectedOption(this);
    }

    // Method to apply wrong answer style
    public void SetWrongAnswerStyle()
    {
        wordText.color = wrongAnswerColor; // Change text color to red
        outlineComponent.enabled = true; // Enable outline for visual feedback
        outlineComponent.effectColor = wrongAnswerColor; // Set outline color to red
        outlineComponent.effectDistance = new Vector2(1, 1); // Optional: Adjust outline distance
    }

    // Method to reset style for the next question
    public void ResetStyle()
    {
        wordText.color = defaultColor; // Reset text color to default
        outlineComponent.enabled = false; // Disable outline
    }
}
