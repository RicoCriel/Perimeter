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

    [Header("Animal related properties")]
    public List<GameObject> activeChickens = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Subscribe the score manager to handle the score decrease event
        weaponBoxController.OnScoreDecrease.AddListener((int amount) =>
        {
            scoreManager.DecreaseScore(amount, scoreText);
        });
    }

    public void AddChicken(GameObject chicken)
    {
        activeChickens.Add(chicken);
    }

    public void RemoveChicken(GameObject chicken)
    {
        activeChickens.Remove(chicken);
    }

    public GameObject GetRandomChicken()
    {
        if (activeChickens.Count == 0)
            return null;

        int randomIndex = Random.Range(0, activeChickens.Count);
        return activeChickens[randomIndex];
    }
}
