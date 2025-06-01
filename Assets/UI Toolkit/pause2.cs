using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class pause2 : MonoBehaviour
{
    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    private bool isPanelVisible = false; // Track the visibility state of the panel
    private bool isGamePaused = false; // Track the game's pause state

    public void TogglePausePanel()
    {
        if (isGamePaused)
        {
            HidePausePanel();
            ResumeGame();
        }
        else
        {
            ShowPausePanel();
            PauseGame();
        }
    }

    public void ShowPausePanel()
    {
        isPanelVisible = true;
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, 0f), fadeTime, false).SetEase(Ease.OutElastic);
        canvasGroup.DOFade(1, fadeTime);
    }

    public void HidePausePanel()
    {
        isPanelVisible = false;
        canvasGroup.alpha = 0f;
        rectTransform.transform.localPosition = new Vector3(0f, -1800f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, -1800f), fadeTime, false).SetEase(Ease.InOutQuint);
        canvasGroup.DOFade(0, fadeTime);
    }

    private void PauseGame()
    {
        if (SceneManager.GetActiveScene().name == "GameplayTwo") // Check if the active scene is named "Gameplay"
        {
            Time.timeScale = 0; // Pause the game by setting the time scale to 0
            isGamePaused = true;
        }
    }

    private void ResumeGame()
    {
        if (SceneManager.GetActiveScene().name == "GameplayTwo") // Check if the active scene is named "Gameplay"
        {
            Time.timeScale = 1; // Resume the game by setting the time scale to 1
            isGamePaused = false;
        }
    }
}
