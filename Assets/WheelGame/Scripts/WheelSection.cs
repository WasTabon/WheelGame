using UnityEngine;
using DG.Tweening;
using System;

public class WheelSection : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer fragmentRenderer;
    public SpriteRenderer highlightRenderer;

    [Header("Section Data")]
    public int sectionIndex;
    public bool isEmpty = true;
    public bool isCorrectFragment;
    public int fragmentId = -1;

    public event Action<WheelSection> OnSectionClicked;

    private Color originalBgColor;
    private bool isClickable = true;
    private Tween highlightTween;
    private Tween fragmentTween;

    private void Awake()
    {
        if (backgroundRenderer != null)
            originalBgColor = backgroundRenderer.color;
    }

    private void OnMouseDown()
    {
        if (!isClickable || isEmpty) return;

        if (IsPointerOverUI()) return;

        AnimateClick();
        
        OnSectionClicked?.Invoke(this);
    }

    private bool IsPointerOverUI()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null) return false;

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0)
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return false;
    }

    public void SetFragment(Sprite fragmentSprite, int fragId, bool correct)
    {
        isEmpty = false;
        isCorrectFragment = correct;
        fragmentId = fragId;

        if (fragmentRenderer != null)
        {
            fragmentRenderer.sprite = fragmentSprite;
            fragmentRenderer.color = new Color(1, 1, 1, 0);
            fragmentRenderer.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
            fragmentRenderer.transform.localScale = Vector3.one * 0.5f;
            fragmentRenderer.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }
    }

    public void ClearFragment()
    {
        if (fragmentRenderer != null && !isEmpty)
        {
            fragmentTween?.Kill();
            fragmentTween = DOTween.Sequence()
                .Append(fragmentRenderer.DOFade(0f, 0.2f))
                .Join(fragmentRenderer.transform.DOScale(Vector3.one * 0.3f, 0.2f).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    fragmentRenderer.sprite = null;
                    fragmentRenderer.transform.localScale = Vector3.one;
                });
        }

        isEmpty = true;
        isCorrectFragment = false;
        fragmentId = -1;
    }

    public void AnimateClick()
    {
        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 8, 0.5f);
    }

    public void AnimateCorrect()
    {
        if (highlightRenderer != null)
        {
            highlightTween?.Kill();
            highlightRenderer.color = new Color(0.2f, 1f, 0.4f, 0.6f);
            highlightTween = highlightRenderer.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    public void AnimateWrong()
    {
        if (highlightRenderer != null)
        {
            highlightTween?.Kill();
            highlightRenderer.color = new Color(1f, 0.2f, 0.2f, 0.6f);
            highlightTween = highlightRenderer.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);
        }

        transform.DOKill(true);
        transform.DOShakePosition(0.4f, 0.15f, 20, 90, false, true);
    }

    public void SetClickable(bool clickable)
    {
        isClickable = clickable;
    }

    public void SetHighlightColor(Color color)
    {
        if (highlightRenderer != null)
            highlightRenderer.color = color;
    }
}
