using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{

    public static ScenesManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum Scene
    {
        MainMenuTwo,
        GuessTheObject,
        MatchingGame,
        MathSkills,
        TrueOrFalse,
        WordPuzzle,
    }

    public void LoadScene (Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNewGame()
    {
        SceneManager.LoadScene(Scene.GuessTheObject.ToString());
    }

    public void LoadWordPuzzle()
    {
        SceneManager.LoadScene(Scene.WordPuzzle.ToString());
    }

    public void LoadMathSkills()
    {
        SceneManager.LoadScene(Scene.MathSkills.ToString());
    }

    public void LoadMatchingGame()
    {
        SceneManager.LoadScene(Scene.MatchingGame.ToString());
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Scene.MainMenuTwo.ToString());
    }

}
