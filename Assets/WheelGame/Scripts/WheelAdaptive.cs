using UnityEngine;

public class WheelAdaptive : MonoBehaviour
{
    public float screenWidthPercent = 0.88f;
    public float wheelWorldRadius = 2.75f;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        Debug.Assert(mainCam != null, "WheelAdaptive: Main camera not found!");
        AdjustScale();
    }

    private void AdjustScale()
    {
        float camHeight = mainCam.orthographicSize * 2f;
        float camWidth = camHeight * mainCam.aspect;

        float targetDiameter = camWidth * screenWidthPercent;
        float currentDiameter = wheelWorldRadius * 2f * transform.localScale.x;

        float scaleFactor = targetDiameter / (wheelWorldRadius * 2f);

        transform.localScale = Vector3.one * scaleFactor;
    }
}
