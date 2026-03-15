using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class WheelCenter : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer contourRenderer;
    public SpriteRenderer glowRenderer;

    private Tween glowTween;
    private int collectedCount;
    private List<SpriteRenderer> collectedRenderers = new List<SpriteRenderer>();

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
            .Append(contourRenderer.DOFade(0.4f, 0.5f).SetEase(Ease.OutQuad))
            .Join(contourRenderer.transform.DOScale(Vector3.one * 1.1f, 0.3f).SetEase(Ease.OutBack))
            .Append(contourRenderer.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuad));
    }

    public void OnFragmentCollected(Sprite fragmentSprite)
    {
        PunchCollect();
        collectedCount++;
        ShowCollectedFragment(fragmentSprite);
    }

    private void ShowCollectedFragment(Sprite fragmentSprite)
    {
        GameObject fragObj = new GameObject("CollectedFrag_" + collectedCount);
        fragObj.transform.SetParent(contourRenderer.transform, false);
        fragObj.transform.localPosition = Vector3.zero;
        fragObj.transform.localScale = Vector3.one;

        SpriteRenderer sr = fragObj.AddComponent<SpriteRenderer>();
        sr.sprite = fragmentSprite;
        sr.sortingOrder = contourRenderer.sortingOrder + collectedCount;
        sr.color = new Color(0.2f, 1f, 0.4f, 0f);

        collectedRenderers.Add(sr);

        DOTween.Sequence()
            .Append(sr.DOFade(0.85f, 0.35f).SetEase(Ease.OutQuad))
            .Join(fragObj.transform.DOScale(Vector3.one * 1.15f, 0.2f).SetEase(Ease.OutBack))
            .Append(fragObj.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.InOutQuad));
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

        foreach (SpriteRenderer sr in collectedRenderers)
        {
            if (sr != null)
                sr.DOColor(new Color(0.3f, 1f, 0.5f, 1f), 0.4f);
        }
    }

    public void ResetCenter()
    {
        collectedCount = 0;

        foreach (SpriteRenderer sr in collectedRenderers)
        {
            if (sr != null)
                Destroy(sr.gameObject);
        }
        collectedRenderers.Clear();

        if (contourRenderer != null)
        {
            contourRenderer.sprite = null;
            contourRenderer.color = new Color(1, 1, 1, 0);
            contourRenderer.transform.localScale = Vector3.one;
        }
        transform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        glowTween?.Kill();
    }
}
