using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxController : MonoBehaviour
{
    private Canvas _weaponCanvas;
    private bool _playerInTrigger = false;
    private bool _openEventTriggered;
    private WeaponBox _weaponBox;

    public UnityEvent OnWeaponBoxInteract;
    public UnityEvent OnWeaponBoxBuy;

    private void Start()
    {
        _weaponCanvas = GetComponentInChildren<Canvas>();
        _weaponBox = GetComponent<WeaponBox>(); 
        _weaponCanvas.enabled = false;
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
            else if (_weaponBox.IsWeaponCycleDone)
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
            Debug.Log("DisplayUI");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = false;
            _openEventTriggered = false;
            Debug.Log("HideUI");
        }
    }
}
