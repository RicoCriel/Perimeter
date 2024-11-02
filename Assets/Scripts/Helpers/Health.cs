using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth;

    private int _decreaseAmount = 30;
    private int _increaseAmount;

    public int CurrentHealth
    {
        get { return _currentHealth; }
        private set { _currentHealth = Mathf.Max(0,value); }
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
        _currentHealth -= _decreaseAmount;
        _currentHealth = Mathf.Max(0, _currentHealth);
    }

    protected void InCreaseHealth()
    {
        _currentHealth += _increaseAmount;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth);
    }
}
