using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollrectLimiter : MonoBehaviour
{
    public ScrollRect scrollRect;

    void LateUpdate()
    {
        // Get the content's RectTransform
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // Calculate the boundaries
        float minX = 0; // Left boundary
        float maxX = Mathf.Max(0, content.rect.width - viewport.rect.width); // Right boundary

        float minY = 0; // Top boundary
        float maxY = Mathf.Max(0, content.rect.height - viewport.rect.height); // Bottom boundary

        // Get the current anchored position
        Vector2 position = content.anchoredPosition;

        // Clamp the position
        position.x = Mathf.Clamp(position.x, -maxX, -minX); // Horizontal
        position.y = Mathf.Clamp(position.y, -maxY, -minY); // Vertical

        // Apply the clamped position
        content.anchoredPosition = position;
    }
}