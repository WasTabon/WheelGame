using UnityEngine;
using System;

public class MusicReactor : MonoBehaviour
{
    public static MusicReactor Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Spectrum Settings")]
    public int spectrumSamples = 256;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Beat Detection")]
    public int bassRangeEnd = 12;
    public float beatThreshold = 0.15f;
    public float beatCooldown = 0.2f;

    [Header("Smoothing")]
    public float energySmoothSpeed = 5f;

    public event Action OnBeat;

    private float[] spectrum;
    private float currentBassEnergy;
    private float smoothBassEnergy;
    private float averageEnergy;
    private float smoothAverageEnergy;
    private float lastBeatTime;
    private float beatHistory;
    private float adaptiveThreshold;

    public float BassEnergy => smoothBassEnergy;
    public float AverageEnergy => smoothAverageEnergy;
    public float NormalizedBass => Mathf.Clamp01(smoothBassEnergy / Mathf.Max(adaptiveThreshold, 0.01f));
    public bool IsPlaying => audioSource != null && audioSource.isPlaying;

    private void Awake()
    {
        Instance = this;
        spectrum = new float[spectrumSamples];
        adaptiveThreshold = beatThreshold;
    }

    private void Start()
    {
        if (audioSource == null && AudioManager.Instance != null)
        {
            audioSource = AudioManager.Instance.musicSource;
            Debug.Log("MusicReactor: Wired to AudioManager.musicSource at runtime");
        }
    }

    private void Update()
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        audioSource.GetSpectrumData(spectrum, 0, fftWindow);

        CalculateEnergies();
        DetectBeat();
    }

    private void CalculateEnergies()
    {
        currentBassEnergy = 0f;
        for (int i = 0; i < bassRangeEnd; i++)
        {
            currentBassEnergy += spectrum[i];
        }

        float totalEnergy = 0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            totalEnergy += spectrum[i];
        }
        averageEnergy = totalEnergy / spectrum.Length;

        smoothBassEnergy = Mathf.Lerp(smoothBassEnergy, currentBassEnergy, Time.deltaTime * energySmoothSpeed);
        smoothAverageEnergy = Mathf.Lerp(smoothAverageEnergy, averageEnergy, Time.deltaTime * energySmoothSpeed);

        beatHistory = Mathf.Lerp(beatHistory, currentBassEnergy, Time.deltaTime * 1.5f);
        adaptiveThreshold = Mathf.Max(beatThreshold, beatHistory * 1.3f);
    }

    private void DetectBeat()
    {
        if (Time.time - lastBeatTime < beatCooldown) return;

        if (currentBassEnergy > adaptiveThreshold)
        {
            lastBeatTime = Time.time;
            OnBeat?.Invoke();
        }
    }

    public float GetFrequencyBand(int bandStart, int bandEnd)
    {
        float energy = 0f;
        bandEnd = Mathf.Min(bandEnd, spectrum.Length);
        for (int i = bandStart; i < bandEnd; i++)
        {
            energy += spectrum[i];
        }
        return energy;
    }
}
