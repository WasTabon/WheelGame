using UnityEngine;
using DG.Tweening;

public class WheelMusicSync : MonoBehaviour
{
    public static WheelMusicSync Instance { get; private set; }

    [Header("References")]
    public WheelController wheelController;
    public MusicReactor musicReactor;

    [Header("Rotation")]
    public float minRotationSpeed = 15f;
    public float maxRotationSpeed = 55f;
    public float rotationImpulseOnBeat = 12f;
    public float rotationReturnSpeed = 3f;

    [Header("Pulse")]
    public float basePulseScale = 1f;
    public float beatPulseAmount = 0.07f;
    public float energyPulseAmount = 0.02f;
    public float pulseReturnSpeed = 6f;

    [Header("Glow")]
    public float glowIntensityMultiplier = 2f;

    private float currentPulse;
    private float impulseDecay;
    private bool isSyncing;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (musicReactor == null)
            musicReactor = MusicReactor.Instance;

        if (wheelController == null)
            wheelController = WheelController.Instance;

        SubscribeToReactor();
    }

    private void SubscribeToReactor()
    {
        if (musicReactor != null)
        {
            musicReactor.OnBeat -= HandleBeat;
            musicReactor.OnBeat += HandleBeat;
        }
    }

    private void OnEnable()
    {
        SubscribeToReactor();
    }

    private void OnDisable()
    {
        if (musicReactor != null)
        {
            musicReactor.OnBeat -= HandleBeat;
        }
    }

    private void Update()
    {
        if (wheelController == null || musicReactor == null) return;
        if (!musicReactor.IsPlaying) return;

        isSyncing = true;

        UpdateRotationSpeed();
        UpdatePulse();
    }

    private void UpdateRotationSpeed()
    {
        float energy = musicReactor.AverageEnergy;
        float normalizedEnergy = Mathf.Clamp01(energy * 15f);

        float targetSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, normalizedEnergy);

        impulseDecay = Mathf.Lerp(impulseDecay, 0f, Time.deltaTime * rotationReturnSpeed);
        targetSpeed += impulseDecay;

        wheelController.SetRotationSpeed(targetSpeed);
    }

    private void UpdatePulse()
    {
        float bassNorm = musicReactor.NormalizedBass;
        float energyPulse = bassNorm * energyPulseAmount;

        currentPulse = Mathf.Lerp(currentPulse, energyPulse, Time.deltaTime * pulseReturnSpeed);

        float finalScale = basePulseScale + currentPulse;
        wheelController.SetExternalPulse(finalScale);
    }

    private void HandleBeat()
    {
        impulseDecay += rotationImpulseOnBeat;

        currentPulse = beatPulseAmount;

        if (wheelController != null && wheelController.wheelCenter != null)
        {
            var glow = wheelController.wheelCenter.glowRenderer;
            if (glow != null)
            {
                glow.DOKill();
                Color c = glow.color;
                glow.color = new Color(c.r, c.g, c.b, 0.7f);
                glow.DOFade(0.2f, 0.4f).SetEase(Ease.OutQuad);
            }
        }
    }

    public void SetSlowMotion(bool active, float multiplier = 0.4f)
    {
        if (active)
        {
            minRotationSpeed *= multiplier;
            maxRotationSpeed *= multiplier;
            rotationImpulseOnBeat *= multiplier;
            beatPulseAmount *= 0.5f;
        }
        else
        {
            minRotationSpeed = 15f;
            maxRotationSpeed = 55f;
            rotationImpulseOnBeat = 12f;
            beatPulseAmount = 0.07f;
        }
    }

    public bool IsSyncing()
    {
        return isSyncing && musicReactor != null && musicReactor.IsPlaying;
    }
}
