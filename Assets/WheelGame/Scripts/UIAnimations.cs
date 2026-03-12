using UnityEngine;
using DG.Tweening;

public static class UIAnimations
{
    public static Tween ScalePunch(Transform target, float punch = 0.15f, float duration = 0.3f)
    {
        target.localScale = Vector3.one;
        return target.DOPunchScale(Vector3.one * punch, duration, 6, 0.5f)
            .SetEase(Ease.OutBack);
    }

    public static Tween ScaleBounceIn(Transform target, float duration = 0.4f, float delay = 0f)
    {
        target.localScale = Vector3.zero;
        return target.DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack)
            .SetDelay(delay);
    }

    public static Tween ScaleBounceOut(Transform target, float duration = 0.3f, float delay = 0f)
    {
        return target.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack)
            .SetDelay(delay);
    }

    public static Tween FadeIn(CanvasGroup group, float duration = 0.35f, float delay = 0f)
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
        return group.DOFade(1f, duration)
            .SetEase(Ease.OutQuad)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                group.interactable = true;
                group.blocksRaycasts = true;
            });
    }

    public static Tween FadeOut(CanvasGroup group, float duration = 0.3f, float delay = 0f)
    {
        group.interactable = false;
        group.blocksRaycasts = false;
        return group.DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .SetDelay(delay);
    }

    public static Tween SlideInFromBottom(RectTransform target, float distance = 200f, float duration = 0.5f, float delay = 0f)
    {
        Vector2 endPos = target.anchoredPosition;
        target.anchoredPosition = endPos + Vector2.down * distance;
        return target.DOAnchorPos(endPos, duration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay);
    }

    public static Tween SlideOutToBottom(RectTransform target, float distance = 200f, float duration = 0.4f, float delay = 0f)
    {
        Vector2 startPos = target.anchoredPosition;
        return target.DOAnchorPos(startPos + Vector2.down * distance, duration)
            .SetEase(Ease.InCubic)
            .SetDelay(delay);
    }

    public static Tween SlideInFromTop(RectTransform target, float distance = 200f, float duration = 0.5f, float delay = 0f)
    {
        Vector2 endPos = target.anchoredPosition;
        target.anchoredPosition = endPos + Vector2.up * distance;
        return target.DOAnchorPos(endPos, duration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay);
    }

    public static Tween FloatUpDown(Transform target, float amplitude = 10f, float duration = 2f)
    {
        return target.DOMoveY(target.position.y + amplitude, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public static Sequence StaggeredAppear(Transform[] elements, float staggerDelay = 0.08f, float duration = 0.35f)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].localScale = Vector3.zero;
            seq.Insert(i * staggerDelay, elements[i].DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
        }
        return seq;
    }
}
