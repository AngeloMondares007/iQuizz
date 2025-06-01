using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LeaderboardPanel : MonoBehaviour
{
    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    private bool isPanelVisible = false; // Track the visibility state of the panel
   

    public void TogglePausePanel()
    {
        if (isPanelVisible)
        {
            HidePausePanel();
        
        }
        else
        {
            ShowPausePanel();
     
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

}
