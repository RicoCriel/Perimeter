using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class WeaponBoxController : MonoBehaviour
{
    private enum WeaponBoxState
    {
        Closed,
        Open,
        Closing,
    }

    private WeaponBoxState _currentState = WeaponBoxState.Closed;

    [Header("Interaction related events")]
    public UnityEvent OnWeaponBoxInteract;
    public UnityEvent OnWeaponBoxBuy;
    public UnityEvent OnWeaponBoxEnter;
    public UnityEvent OnWeaponBoxExit;
    public UnityEvent OnWeaponBoxClosing;

    [Header("Ui related events")]
    public UnityEvent<int> OnScoreDecrease;

    private bool _playerInTrigger;

    private void Update()
    {
        if (_playerInTrigger)
        {
            HandleInteraction();
            HandleWeaponBuying();
        }
    }

    public void HandleInteraction()
    {
        if (_currentState == WeaponBoxState.Closed && WeaponBox.Instance.ShouldInteract)
        {
            if (ScoreManager.Instance.Score >= WeaponBox.Instance.WeaponBoxPrice)
            {
                OnWeaponBoxInteract?.Invoke();
                OnScoreDecrease?.Invoke(WeaponBox.Instance.WeaponBoxPrice);
                UpdateWeaponBoxState(WeaponBoxState.Open);
            }
            else if (ScoreManager.Instance.Score < WeaponBox.Instance.WeaponBoxPrice)
            {
                OnScoreDecrease?.Invoke(0);
                return;
            }
        }
    }

    public void HandleWeaponBuying()
    {
        if (_currentState == WeaponBoxState.Open && WeaponBox.Instance.ShouldInteract)
        {
            if (WeaponBox.Instance.CanBuyWeapon)
            {
                OnWeaponBoxBuy?.Invoke();
                OnScoreDecrease?.Invoke(WeaponBox.Instance.WeaponBoxPrice);
                UpdateWeaponBoxState(WeaponBoxState.Closing);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = true;
            if(_currentState == WeaponBoxState.Closed)
            {
                OnWeaponBoxEnter?.Invoke();
            }
            else if(_currentState == WeaponBoxState.Closing)
            {
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = false;
            OnWeaponBoxExit?.Invoke();
        }
    }

    private void ResetWeaponBoxState()
    {
        UpdateWeaponBoxState(WeaponBoxState.Closed);
        _playerInTrigger = false;
    }

    private void UpdateWeaponBoxState(WeaponBoxState newState)
    {
        _currentState = newState;
    }

    public void SetWeaponBoxClosingState()
    {
        UpdateWeaponBoxState(WeaponBoxState.Closing);
    }
}
