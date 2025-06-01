using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    private bool isPanelVisible = false; // Track the visibility state of the panel

    public void ToggleSettingsPanel()
    {
        if (isPanelVisible)
        {
            HideSettingsPanel();
        }
        else
        {
            ShowSettingsPanel();
        }
    }

    public void ShowSettingsPanel()
    {
        isPanelVisible = true;
        canvasGroup.alpha = 0f;
        rectTransform.transform.localPosition = new Vector3(0f, 300f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, 0f), fadeTime, false).SetEase(Ease.OutElastic);
        canvasGroup.DOFade(1, fadeTime);
    }

    public void HideSettingsPanel()
    {
        isPanelVisible = false;
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, -1800f), fadeTime, false).SetEase(Ease.InOutQuint);
        canvasGroup.DOFade(0, fadeTime);
    }
}
