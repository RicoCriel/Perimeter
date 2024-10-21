using TMPro;
using DG.Tweening;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    private int _zombieHitPoint = 15;
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

    public void IncreaseScore(int amount, TextMeshProUGUI scoreText)
    {
        AnimateScore(Score, Score + amount, scoreText);
        Score += amount; 
        UpdateScore(Score, scoreText);
    }

    public void DecreaseScore(int amount, TextMeshProUGUI scoreText)
    {
        AnimateScore(Score, Mathf.Max(0, Score - amount), scoreText);
        Score -= amount; 
        UpdateScore(Score, scoreText);
    }

    private void AnimateScore(int fromValue, int toValue, TextMeshProUGUI scoreText)
    {
        // Determine color based on whether the score is increasing or decreasing
        Color targetColor = (toValue > fromValue) ? Color.green : Color.red;

        // Animate the score value and scale
        DOTween.To(() => fromValue, x => scoreText.text = x.ToString(), toValue, 1f) 
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                scoreText.DOColor(Color.white, 0.2f);
            });

        scoreText.DOColor(targetColor, 0.2f);

        // Animate the scale of the text
        RectTransform scoreRectTransform = scoreText.GetComponent<RectTransform>();
        scoreRectTransform.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad) 
            .OnComplete(() =>
            {
                scoreRectTransform.DOScale(1f, 0.2f).SetEase(Ease.InQuad); 
            });
    }

    private void UpdateScore(int score, TextMeshProUGUI scoreText)
    {
        scoreText.text = Score.ToString();
    }
}
