using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxController : MonoBehaviour
{
    private bool _playerInTrigger = false;
    private bool _openEventTriggered;
    private WeaponBox _weaponBox;
    private bool _weaponPurchased = false;  

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
        // Only trigger interaction if the box is ready to open (and not purchased yet)
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

       // Only allow purchasing a weapon after the cycle completes
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
            
            if(_weaponPurchased)
            {
                ResetWeaponBox();
            }
        }
    }

    private void ResetWeaponBox()
    {
        _weaponPurchased = false;  // Allow buying another weapon after a full cycle
        _openEventTriggered = false;  // Allow the box to be opened again on re-entry
    }

    
}
