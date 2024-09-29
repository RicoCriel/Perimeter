using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxController : MonoBehaviour
{
    private bool _playerInTrigger = false;
    private bool _openEventTriggered;
    private WeaponBox _weaponBox;
    private bool _weaponPurchased = false;
    private Coroutine _resetCooldownCoroutine;

    [Header("Interaction related events")]
    public UnityEvent OnWeaponBoxInteract;
    public UnityEvent OnWeaponBoxBuy;
    public UnityEvent OnWeaponBoxEnter;
    public UnityEvent OnWeaponBoxExit;

    [Header("Ui related events")]
    public UnityEvent<int> OnScoreDecrease;

    private void Awake()
    {
        _weaponBox = GetComponent<WeaponBox>();
    }

    private void Update()
    {
        if (_playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (!_openEventTriggered && !_weaponPurchased)
        {
            if (ScoreManager.Instance.Score >= _weaponBox.WeaponBoxPrice)
            {
                OnScoreDecrease?.Invoke(_weaponBox.WeaponBoxPrice);
                OnWeaponBoxInteract?.Invoke();  
                _openEventTriggered = true;
            }
            else
            {
                OnScoreDecrease?.Invoke(0);
            }
        }

       if (_weaponBox.IsWeaponCycleDone && !_weaponPurchased)
       {
            OnWeaponBoxBuy?.Invoke();  
            _weaponPurchased = true;   
       }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = true;
            OnWeaponBoxEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = false;
            OnWeaponBoxExit?.Invoke();

            if (_weaponPurchased)
            {
                ResetWeaponBox();
            }
            else if (!_weaponPurchased)
            {
                if(_resetCooldownCoroutine != null)
                {
                    StopCoroutine(_resetCooldownCoroutine);
                }
                _resetCooldownCoroutine = StartCoroutine(ResetWeaponBoxCooldown());
            }
        }
    }

    private void ResetWeaponBox()
    {
        _weaponPurchased = false;  
        _openEventTriggered = false;  
    }

    private IEnumerator ResetWeaponBoxCooldown()
    {
        yield return new WaitForSeconds(2f);  

        ResetWeaponBox();  
        _resetCooldownCoroutine = null;  
    }
}
