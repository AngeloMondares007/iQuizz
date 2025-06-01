using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableQuestion : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;

    void Start()
    {
        originalPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optionally, you can reset the position if it is not dropped on an option
        // You may also want to call a method to check if it was dropped on a correct option
    }

    public Vector2 GetOriginalPosition()
    {
        return originalPosition;
    }
}
