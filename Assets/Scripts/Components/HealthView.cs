using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthView : MonoBehaviour
{
    private Health _health;
    private Image _healthBar;

    void Start()
    {
        _health = FindObjectOfType<Health>();
        _healthBar = GetComponentInChildren<Image>(); 
    }

    void Update()
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if(_health.IsPlayer)
        {
            _healthBar.fillAmount = _health.CurrentHealth;
        }
    }
}
