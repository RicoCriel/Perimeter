using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Image _crosshairImage;
    [SerializeField] private GameObject _player;  
    [SerializeField] private float _minDistanceFromPlayer;
    [SerializeField] private float _maxRange;
    [SerializeField] private float _smoothingSpeed;

    private RectTransform _canvasRect;  
    private RectTransform _crosshairRectTransform;  

    void Start()
    {
        Cursor.visible = false;  
        _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();  
        _crosshairRectTransform = GetComponent<RectTransform>();  
    }

    void Update()
    {
        MoveCrossHair();
        CheckForEnemyUnderCrosshair();
    }

    private void MoveCrossHair()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            Input.mousePosition,
            _canvasRect.GetComponent<Canvas>().worldCamera,
            out localPoint
        );

        // Convert the player's world position to the canvas's local position
        Vector2 playerLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            Camera.main.WorldToScreenPoint(_player.transform.position),
            _canvasRect.GetComponent<Canvas>().worldCamera,
            out playerLocalPoint
        );

        // Calculate the distance from the player's position
        Vector2 toCrosshair = localPoint - playerLocalPoint;
        float distanceFromPlayer = toCrosshair.magnitude;

        // Ensure the crosshair is at least minDistance from the player
        if (distanceFromPlayer < _minDistanceFromPlayer)
        {
            toCrosshair = toCrosshair.normalized * _minDistanceFromPlayer;
            localPoint = playerLocalPoint + toCrosshair;
        }
        
        // Clamp the distance based on maxRange
        if (distanceFromPlayer > _maxRange)
        {
            toCrosshair = toCrosshair.normalized * _maxRange;
            localPoint = playerLocalPoint + toCrosshair;
        }

        // Further clamp the local point to ensure the crosshair stays within the canvas bounds
        Vector2 crosshairSize = _crosshairRectTransform.sizeDelta;
        float clampedX = Mathf.Clamp(localPoint.x, -_canvasRect.rect.width / 2 + crosshairSize.x / 2, _canvasRect.rect.width / 2 - crosshairSize.x / 2);
        float clampedY = Mathf.Clamp(localPoint.y, -_canvasRect.rect.height / 2 + crosshairSize.y / 2, _canvasRect.rect.height / 2 - crosshairSize.y / 2);

        Vector2 targetPosition = new Vector2(clampedX, clampedY);
        _crosshairRectTransform.anchoredPosition = Vector3.Lerp(_crosshairRectTransform.anchoredPosition, targetPosition, Time.deltaTime * _smoothingSpeed);
    }

    private void CheckForEnemyUnderCrosshair()
    {
        // Convert crosshair position to world point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_canvasRect.GetComponent<Canvas>().worldCamera, _crosshairRectTransform.position);
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetComponent<Enemy>() != null)
            {
                _crosshairImage.color = Color.red;
            }
            else
            {
                _crosshairImage.color = Color.white;
            }
        }
        else
        {
            _crosshairImage.color = Color.white;
        }
    }

}
