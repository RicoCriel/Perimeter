using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxController : MonoBehaviour
{
    private bool _playerInTrigger = false;
    private bool _openEventTriggered;
    private WeaponBox _weaponBox;

    public UnityEvent OnWeaponBoxInteract;
    public UnityEvent OnWeaponBoxBuy;

    private void Start()
    {
        _weaponBox = GetComponent<WeaponBox>(); 
    }

    private void Update()
    {
        if (_playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (!_openEventTriggered)
            {
                OnWeaponBoxInteract?.Invoke();
                _openEventTriggered = true;
                
            }

            if (_weaponBox.IsWeaponCycleDone)
            {
                OnWeaponBoxBuy?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = false;
            _openEventTriggered = false;
        }
    }
}
