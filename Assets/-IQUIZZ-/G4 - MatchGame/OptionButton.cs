using UnityEngine;
using UnityEngine.UI;

[System.Serializable] // This is needed if you want to create a list of Options
public class OptionButton : MonoBehaviour
{
    [SerializeField]
    private Sprite correctAnswer; // Mark this as Serializable

    [SerializeField]
    private Image buttonImage; // Reference to the button image component

    // Method to set the correct answer
    public void SetCorrectAnswer(Sprite answer)
    {
        correctAnswer = answer;
        if (buttonImage != null)
        {
            buttonImage.sprite = correctAnswer; // Update the button image
        }
    }

    // Add any other necessary methods or properties
}
