using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Vector3 originalScale;
    private Tween currentTween;
    private bool isPressed;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * 0.92f, 0.1f)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, 0.15f)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        currentTween?.Kill();
        transform.localScale = originalScale;
        currentTween = transform.DOPunchScale(Vector3.one * 0.08f, 0.25f, 8, 0.5f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }

    private void OnDisable()
    {
        currentTween?.Kill();
        transform.localScale = originalScale;
        isPressed = false;
    }
}
