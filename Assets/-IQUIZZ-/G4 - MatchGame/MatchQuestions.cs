using UnityEngine;


[System.Serializable]
public class MatchQuestions
{
    public string questionText; // Text for the question
    public Sprite questionImage; // Image for the question
    public Sprite[] optionImages = new Sprite[4]; // Array of 4 images for the answer options
    public string[] optionTexts; // Texts for the options
    public Sprite correctAnswer; // Correct answer image
}
