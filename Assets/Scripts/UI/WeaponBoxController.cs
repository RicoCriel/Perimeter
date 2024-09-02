using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxController : MonoBehaviour
{
    private Canvas _weaponCanvas;
    private bool _playerInTrigger = false;
    private bool _eventTriggered = false;

    public UnityEvent OnWeaponBoxInteract;  

    private void Start()
    {
        _weaponCanvas = GetComponentInChildren<Canvas>();
        _weaponCanvas.enabled = false;
    }

    private void Update()
    {
        if (_playerInTrigger && !_eventTriggered && Input.GetKeyDown(KeyCode.E))
        {
            OnWeaponBoxInteract?.Invoke();  
            _eventTriggered = true;  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = true;
            _eventTriggered = false;  
            Debug.Log("DisplayUI");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = false;
            Debug.Log("HideUI");
        }
    }
}
