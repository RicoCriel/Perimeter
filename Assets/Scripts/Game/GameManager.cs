using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score related properties")]
    public WeaponBoxController weaponBoxController;
    public ScoreManager scoreManager;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        // Subscribe the score manager to handle the score decrease event
        weaponBoxController.OnScoreDecrease.AddListener((int amount) =>
        {
            scoreManager.DecreaseScore(amount, scoreText);
        });
    }
}
