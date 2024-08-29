using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    private RectTransform canvasRect;
    private RectTransform crosshairRectTransform;

    void Start()
    {
        Cursor.visible = false;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        crosshairRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        MoveCrossHair();
    }

    private void MoveCrossHair()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvasRect.GetComponent<Canvas>().worldCamera,
            out localPoint
        );

        // Calculate the size of the crosshair to use for clamping
        Vector2 crosshairSize = crosshairRectTransform.sizeDelta;

        // Clamp the local point to ensure the crosshair stays within the canvas bounds
        float clampedX = Mathf.Clamp(localPoint.x, -canvasRect.rect.width / 2 + crosshairSize.x / 2, canvasRect.rect.width / 2 - crosshairSize.x / 2);
        float clampedY = Mathf.Clamp(localPoint.y, -canvasRect.rect.height / 2 + crosshairSize.y / 2, canvasRect.rect.height / 2 - crosshairSize.y / 2);

        // Set the clamped position
        crosshairRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
    }
}
