using UnityEngine;
using DG.Tweening;

public class WheelController : MonoBehaviour
{
    public static WheelController Instance { get; private set; }

    [Header("Wheel Setup")]
    public Transform wheelTransform;
    public WheelSection[] sections = new WheelSection[12];
    public WheelCenter wheelCenter;

    [Header("Rotation")]
    public float baseRotationSpeed = 30f;
    public float currentRotationSpeed;

    [Header("Fallback Pulse")]
    public float basePulseScale = 1f;
    public float pulseAmount = 0.03f;
    public float pulseSpeed = 2f;

    private float rotationAngle;
    private float targetRotationSpeed;
    private float rotationVelocity;
    private float pulseTimer;
    private bool isActive = true;
    private bool useExternalPulse;
    private float externalPulseScale = 1f;

    private void Awake()
    {
        Instance = this;
        currentRotationSpeed = baseRotationSpeed;
        targetRotationSpeed = baseRotationSpeed;
    }

    private void Update()
    {
        if (!isActive) return;

        currentRotationSpeed = Mathf.SmoothDamp(currentRotationSpeed, targetRotationSpeed, ref rotationVelocity, 0.3f);

        rotationAngle += currentRotationSpeed * Time.deltaTime;
        wheelTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        if (useExternalPulse)
        {
            wheelTransform.localScale = Vector3.Lerp(
                wheelTransform.localScale,
                Vector3.one * externalPulseScale,
                Time.deltaTime * 10f
            );
        }
        else
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = basePulseScale + Mathf.Sin(pulseTimer) * pulseAmount;
            wheelTransform.localScale = Vector3.one * pulse;
        }
    }

    public void SetExternalPulse(float scale)
    {
        useExternalPulse = true;
        externalPulseScale = scale;
    }

    public void DisableExternalPulse()
    {
        useExternalPulse = false;
    }

    public void SetRotationSpeed(float speed)
    {
        targetRotationSpeed = speed;
    }

    public void AddRotationImpulse(float impulse)
    {
        currentRotationSpeed += impulse;
    }

    public void PunchScale(float amount = 0.06f, float duration = 0.3f)
    {
        wheelTransform.DOKill(true);
        wheelTransform.DOPunchScale(Vector3.one * amount, duration, 6, 0.5f);
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void ResetWheel()
    {
        rotationAngle = 0f;
        currentRotationSpeed = baseRotationSpeed;
        targetRotationSpeed = baseRotationSpeed;
        wheelTransform.rotation = Quaternion.identity;
        wheelTransform.localScale = Vector3.one * basePulseScale;
        useExternalPulse = false;
    }

    public WheelSection GetSectionByIndex(int index)
    {
        Debug.Assert(index >= 0 && index < sections.Length, "Section index out of range: " + index);
        return sections[index];
    }

    public int GetSectionCount()
    {
        return sections.Length;
    }
}
