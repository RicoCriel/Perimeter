using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    public static ScoreManager Instance;

    public int Score
    {
        get { return _score; }
        private set { _score = Mathf.Max(0, value); } // Ensure score doesn't go below 0
    }

    private void Awake()
    {
        Instance = this;
    }

    public void IncreaseScore(int amount, TextMeshProUGUI scoreText)
    {
        Score += amount;
        UpdateScore(Score, scoreText);
    }

    public void DecreaseScore(int amount, TextMeshProUGUI scoreText)
    {
        Score -= amount;
        UpdateScore(Score, scoreText);
    }

    private void UpdateScore(int score, TextMeshProUGUI scoreText)
    {
        scoreText.text = Score.ToString();
    }
}
