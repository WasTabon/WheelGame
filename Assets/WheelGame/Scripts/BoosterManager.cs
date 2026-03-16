using UnityEngine;
using DG.Tweening;
using System;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance { get; private set; }

    [Header("Slowmo Settings")]
    public float slowmoDuration = 5f;
    public float slowmoPitch = 0.6f;
    public float slowmoLowPassFreq = 800f;

    public event Action OnBoostersChanged;

    private AudioLowPassFilter lowPassFilter;
    private bool isSlowmoActive;
    private float slowmoTimer;
    private Tween slowmoEndTween;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isSlowmoActive)
        {
            slowmoTimer -= Time.deltaTime;
            if (slowmoTimer <= 0f)
            {
                EndSlowmo();
            }
        }
    }

    public bool CanUseUndo()
    {
        return GameManager.Instance.HasBooster(BoosterType.Undo)
            && GameplayManager.Instance != null
            && GameplayManager.Instance.HasUndoData();
    }

    public bool CanUseSlowmo()
    {
        return GameManager.Instance.HasBooster(BoosterType.Slowmo)
            && !isSlowmoActive;
    }

    public bool CanUseExtraLife()
    {
        return GameManager.Instance.HasBooster(BoosterType.ExtraLife);
    }

    public void UseUndo()
    {
        if (!CanUseUndo()) return;

        GameManager.Instance.UseBooster(BoosterType.Undo);
        GameplayManager.Instance.ExecuteUndo();
        OnBoostersChanged?.Invoke();
    }

    public void UseSlowmo()
    {
        if (!CanUseSlowmo()) return;

        GameManager.Instance.UseBooster(BoosterType.Slowmo);
        StartSlowmo();
        OnBoostersChanged?.Invoke();
    }

    public void UseExtraLife()
    {
        if (!CanUseExtraLife()) return;

        GameManager.Instance.UseBooster(BoosterType.ExtraLife);
        GameplayManager.Instance.AddLife();
        OnBoostersChanged?.Invoke();
    }

    private void StartSlowmo()
    {
        isSlowmoActive = true;
        slowmoTimer = slowmoDuration;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicPitch(slowmoPitch);

            if (lowPassFilter == null)
            {
                lowPassFilter = AudioManager.Instance.musicSource.gameObject.GetComponent<AudioLowPassFilter>();
                if (lowPassFilter == null)
                    lowPassFilter = AudioManager.Instance.musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            }
            lowPassFilter.enabled = true;
            lowPassFilter.cutoffFrequency = 22000f;
            DOTween.To(() => lowPassFilter.cutoffFrequency,
                x => lowPassFilter.cutoffFrequency = x,
                slowmoLowPassFreq, 0.5f).SetEase(Ease.OutQuad);
        }

        if (WheelMusicSync.Instance != null)
        {
            WheelMusicSync.Instance.SetSlowMotion(true);
        }
        else if (WheelController.Instance != null)
        {
            WheelController.Instance.SetRotationSpeed(WheelController.Instance.baseRotationSpeed * 0.4f);
        }
    }

    private void EndSlowmo()
    {
        isSlowmoActive = false;

        if (AudioManager.Instance != null)
        {
            DOTween.To(() => AudioManager.Instance.musicSource.pitch,
                x => AudioManager.Instance.musicSource.pitch = x,
                1f, 0.6f).SetEase(Ease.InOutQuad);

            if (lowPassFilter != null)
            {
                DOTween.To(() => lowPassFilter.cutoffFrequency,
                    x => lowPassFilter.cutoffFrequency = x,
                    22000f, 0.6f).SetEase(Ease.InQuad)
                    .OnComplete(() => lowPassFilter.enabled = false);
            }
        }

        if (WheelMusicSync.Instance != null)
        {
            WheelMusicSync.Instance.SetSlowMotion(false);
        }
        else if (WheelController.Instance != null)
        {
            WheelController.Instance.SetRotationSpeed(WheelController.Instance.baseRotationSpeed);
        }
    }

    public bool IsSlowmoActive()
    {
        return isSlowmoActive;
    }

    public float GetSlowmoTimeRemaining()
    {
        return isSlowmoActive ? slowmoTimer : 0f;
    }

    public void ResetSlowmo()
    {
        if (isSlowmoActive)
            EndSlowmo();
    }
}
