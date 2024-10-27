using TMPro;
using DG.Tweening;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    private int _zombieHitPoint = 15;
    [SerializeField] private TextMeshProUGUI _scoreText;
    public static ScoreManager Instance { get; private set; }

    public int Score
    {
        get { return _score; }
        private set { _score = Mathf.Max(0, value); } 
    }

    public int ZombieHitPoint
    {
        get { return _zombieHitPoint; }
        private set { _zombieHitPoint = value; }
    }

    private void Awake()
    {
        if(Instance != null)
        { 
            Destroy(Instance);
        }
        Instance = this;
    }

    public void IncreaseScore(int amount)
    {
        AnimateScore(Score, Score + amount);
        Score += amount; 
        UpdateScore(Score);
    }

    public void DecreaseScore(int amount)
    {
        AnimateScore(Score, Mathf.Max(0, Score - amount));
        Score -= amount; 
        UpdateScore(Score);
    }

    private void AnimateScore(int fromValue, int toValue)
    {
        // Determine color based on whether the score is increasing or decreasing
        Color targetColor = (toValue > fromValue) ? Color.green : Color.red;

        // Animate the score value and scale
        DOTween.To(() => fromValue, x => _scoreText.text = x.ToString(), toValue, 1f) 
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _scoreText.DOColor(Color.white, 0.2f);
            });

        _scoreText.DOColor(targetColor, 0.2f);

        // Animate the scale of the text
        RectTransform scoreRectTransform = _scoreText.GetComponent<RectTransform>();
        scoreRectTransform.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad) 
            .OnComplete(() =>
            {
                scoreRectTransform.DOScale(1f, 0.2f).SetEase(Ease.InQuad); 
            });
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = Score.ToString();
    }
}
