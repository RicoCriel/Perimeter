using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth;

    private int _decreaseAmount = 50;
    private int _increaseAmount;

    public bool IsPlayer;

    public int CurrentHealth
    {
        get { return _currentHealth; }
        private set
        {
            _currentHealth = Mathf.Max(0, value);
        }
    }

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
    }

    private void OnDisable()
    {
        _currentHealth = 0;
    }

    public void DecreaseHealth()
    {
        CurrentHealth -= _decreaseAmount;
    }

    public void IncreaseHealth()
    {
        CurrentHealth += _increaseAmount;
    }
}
