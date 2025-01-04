using UnityEngine;
using UnityEngine.UI;

public class TargetObserver : MonoBehaviour, IAimObserver
{
    public Image AimCrosshair;
    public float CrosshairSpeed;

    public void OnAimStarted()
    {
        AimCrosshair.enabled = true;
    }

    public void OnAimStopped()
    {
        AimCrosshair.enabled = false;
    }

    public void OnAimUpdated(Vector3 targetPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);
        AimCrosshair.rectTransform.position = Vector3.Lerp(
            AimCrosshair.rectTransform.position,
            screenPosition,
            Time.deltaTime * CrosshairSpeed
        );
    }
}
