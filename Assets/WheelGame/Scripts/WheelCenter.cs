using UnityEngine;
using DG.Tweening;

public class WheelCenter : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer contourRenderer;
    public SpriteRenderer glowRenderer;

    [Header("Collected Fragments")]
    public SpriteRenderer[] collectedFragmentSlots;

    private Tween glowTween;
    private int collectedCount;

    private void Start()
    {
        StartGlowAnimation();
    }

    private void StartGlowAnimation()
    {
        if (glowRenderer == null) return;
        glowRenderer.color = new Color(glowRenderer.color.r, glowRenderer.color.g, glowRenderer.color.b, 0.1f);
        glowTween = glowRenderer.DOFade(0.4f, 1.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void SetContour(Sprite contourSprite)
    {
        if (contourRenderer == null)
        {
            Debug.LogWarning("WheelCenter: contourRenderer is null!");
            return;
        }

        contourRenderer.sprite = contourSprite;
        contourRenderer.color = new Color(1, 1, 1, 0);

        DOTween.Sequence()
            .Append(contourRenderer.DOFade(0.6f, 0.5f).SetEase(Ease.OutQuad))
            .Join(contourRenderer.transform.DOScale(Vector3.one * 1.1f, 0.3f).SetEase(Ease.OutBack))
            .Append(contourRenderer.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuad));
    }

    public void OnFragmentCollected(Sprite fragmentSprite)
    {
        PunchCollect();
        collectedCount++;
    }

    public void PunchCollect()
    {
        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * 0.1f, 0.35f, 8, 0.4f);

        if (glowRenderer != null)
        {
            glowRenderer.DOKill();
            glowRenderer.DOFade(0.8f, 0.1f).OnComplete(() =>
            {
                StartGlowAnimation();
            });
        }
    }

    public void AnimateComplete()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack))
            .Append(transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuad));

        if (contourRenderer != null)
            contourRenderer.DOFade(1f, 0.3f);
    }

    public void ResetCenter()
    {
        collectedCount = 0;
        if (contourRenderer != null)
        {
            contourRenderer.sprite = null;
            contourRenderer.color = new Color(1, 1, 1, 0);
        }
        transform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        glowTween?.Kill();
    }
}
