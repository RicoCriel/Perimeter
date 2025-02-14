using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BpmView : MonoBehaviour
{
    [SerializeField] private Image _heartIcon;
    [SerializeField] private float _heartRate = 60f;

    [Header("Heart Rate Thresholds")]
    [SerializeField] private float _lowThreshold = 100f;
    [SerializeField] private float _highThreshold = 140f;

    [Header("Heart Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _warningColor = Color.yellow;
    [SerializeField] private Color _dangerColor = Color.red;

    private Tween _pulseTween;

    private void Start()
    {
        StartHeartbeat();
    }

    public void UpdateHeartRate(float newBpm)
    {
        _heartRate = newBpm;
        UpdateColor();
        RestartHeartbeat();
    }

    private void StartHeartbeat()
    {
        float duration = Mathf.Clamp(60f / _heartRate, 0.3f, 1f); // Faster beats for higher BPM

        _pulseTween = _heartIcon.transform
            .DOScale(1.2f, duration * 0.5f) // Scale up
            .SetLoops(-1, LoopType.Yoyo) // Loop back and forth
            .SetEase(Ease.InOutSine); // Smooth transition
    }

    private void RestartHeartbeat()
    {
        _pulseTween.Kill(); // Stop existing animation
        StartHeartbeat(); // Restart with new BPM
    }

    private void UpdateColor()
    {
        if (_heartRate >= _highThreshold)
        {
            _heartIcon.color = _dangerColor;
        }
        else if (_heartRate >= _lowThreshold)
        {
            _heartIcon.color = _warningColor;
        }
        else
        {
            _heartIcon.color = _normalColor;
        }
    }
}
