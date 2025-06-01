using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class GSQUESTIONS
{
    public string sentence; // The sentence with a blank
    public string correctAnswer; // The correct option (e.g., "is")
    public string[] options; // Two options (e.g., {"is", "are"})
    public Sprite questionImage; // Optional picture for the question

    // Constructor to initialize the question with or without an image
    public GSQUESTIONS(string sentence, string correctAnswer, string[] options, Sprite questionImage = null)
    {
        this.sentence = sentence;
        this.correctAnswer = correctAnswer;
        this.options = options;
        this.questionImage = questionImage; // Can be null if no image is provided
    }
}
