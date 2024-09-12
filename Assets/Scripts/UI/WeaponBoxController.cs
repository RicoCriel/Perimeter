using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxController : MonoBehaviour
{
    private bool _playerInTrigger = false;
    private bool _openEventTriggered;
    private WeaponBox _weaponBox;
    private bool _weaponPurchased = false;  // Tracks if a weapon has been bought

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
                OnWeaponBoxInteract?.Invoke();  // Open the box and start the weapon cycle
                _openEventTriggered = true;
            }
        }

       // Only allow purchasing a weapon after the cycle completes
       if (_weaponBox.IsWeaponCycleDone && !_weaponPurchased)
       {
            OnWeaponBoxBuy?.Invoke();  // Buy the weapon
            _weaponPurchased = true;   // Mark weapon as purchased
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
            // Only reset if the weapon has been purchased
            if (_weaponPurchased)
            {
                ResetWeaponBox();  // Reset the interaction state after the player leaves the area
            }
        }
    }

    // Reset the state so the box can be used again in future interactions
    private void ResetWeaponBox()
    {
        _weaponPurchased = false;  // Allow buying another weapon after a full cycle
        _openEventTriggered = false;  // Allow the box to be opened again on re-entry
    }

    
}
