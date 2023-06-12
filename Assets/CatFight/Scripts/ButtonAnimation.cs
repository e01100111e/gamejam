using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform buttonRectTransform;
    private float originalScale;
    private Button button;

    private void Awake()
    {
        buttonRectTransform = GetComponent<RectTransform>();
        originalScale = buttonRectTransform.localScale.x;
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button!= null && button.interactable)
            buttonRectTransform.DOScale(originalScale * 0.9f, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button != null && button.interactable)
            buttonRectTransform.DOScale(originalScale, 0.1f);
    }
}
