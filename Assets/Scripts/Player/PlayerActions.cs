using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerActions : Player
{
    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintMultiplier;
    [SerializeField] private float _drag;

    [Header("Input Settings")]
    [Range(0.01f, 1f)]
    [SerializeField] private float _sensitivity;
    [SerializeField] private bool _shouldInvertX;

    [Header("Reload Settings")]
    [SerializeField] private bool _shouldAutoReload;

    [Header("Aiming Settings")]
    [SerializeField] private GameObject _aimingReticle;

    [Header("Map Settings")]
    [SerializeField] private Transform _mapCenter;
    [SerializeField] private float _mapRadius;

    public InputActionAsset PrimaryActions;
    private InputActionMap _gameplayActionMap;
    private InputAction _moveInputAction;
    private InputAction _rotateInputAction;
    private InputAction _sprintInputAction;
    private InputAction _singleFireInputAction;
    private InputAction _automaticFireInputAction;
    private InputAction _interactInputAction;
    private InputAction _aimInputAction;
    private InputAction _reloadInputAction;

    private Vector2 _moveInput;
    private float _angle;
    private bool _shouldSprint;

    private Coroutine _autoFireCoroutine; 
    public UnityEvent OnFire;

    private void Awake()
    {
        GetComponents();
        _gameplayActionMap = PrimaryActions.FindActionMap("Gameplay");
        FindInputActions();
        SubscribeInputActionEvents();
    }

    private void OnEnable()
    {
        EnableInputActions(true);
    }

    private void OnDisable()
    {
        UnsubscribeInputActionEvents();
        EnableInputActions(false);
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
    }

    private void FindInputActions()
    {
        _moveInputAction = _gameplayActionMap.FindAction("Move");
        _sprintInputAction = _gameplayActionMap.FindAction("Sprint");
        _singleFireInputAction = _gameplayActionMap.FindAction("Single Fire");
        _automaticFireInputAction = _gameplayActionMap.FindAction("Auto Fire");
        _interactInputAction = _gameplayActionMap.FindAction("Interact");
        _rotateInputAction = _gameplayActionMap.FindAction("Rotate");
        _reloadInputAction = _gameplayActionMap.FindAction("Reload");
        _aimInputAction = _gameplayActionMap.FindAction("Aim");
    }

    private void EnableInputActions(bool isEnabled)
    {
        if (isEnabled)
        {
            _moveInputAction.Enable();
            _sprintInputAction.Enable();
            _singleFireInputAction.Enable();
            _automaticFireInputAction.Enable();
            _interactInputAction.Enable();
            _reloadInputAction.Enable();
            _aimInputAction.Enable();
        }
        else
        {
            _moveInputAction.Disable();
            _sprintInputAction.Disable();
            _singleFireInputAction.Disable();
            _automaticFireInputAction.Disable();
            _interactInputAction.Disable();
            _reloadInputAction.Disable();
            _aimInputAction.Disable();
        }
    }

    private void SubscribeInputActionEvents()
    {
        _sprintInputAction.performed += OnSprintPerformed;
        _moveInputAction.performed += OnMovePerformed;
        _rotateInputAction.performed += OnRotatePerformed;
        _singleFireInputAction.performed += OnSingleFirePerformed;
        _automaticFireInputAction.started += OnAutoFirePerformed;
        _interactInputAction.performed += OnInteractPerformed;
        _reloadInputAction.performed += OnReloadCanceled;
        _aimInputAction.performed += OnAimPerformed;

        _sprintInputAction.canceled += OnSprintPerformed;
        _moveInputAction.canceled += OnMoveCanceled;
        _rotateInputAction.canceled += OnRotateCanceled;
        _singleFireInputAction.canceled += OnSingleFirePerformed;
        _automaticFireInputAction.canceled += OnAutoFireCanceled;
        _interactInputAction.canceled += OnInteractCanceled;
        _reloadInputAction.canceled += OnReloadCanceled;
        _aimInputAction.canceled += OnAimPerformed;
    }

    private void UnsubscribeInputActionEvents()
    {
        _sprintInputAction.performed -= OnSprintPerformed;
        _moveInputAction.performed -= OnMovePerformed;
        _rotateInputAction.performed -= OnRotatePerformed;
        _singleFireInputAction.performed -= OnSingleFirePerformed;
        _automaticFireInputAction.started -= OnAutoFirePerformed;
        _interactInputAction.performed -= OnInteractPerformed;
        _aimInputAction.performed -= OnAimPerformed;    
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    { 
        _angle = context.ReadValue<float>();
    }

    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        _angle = 0f;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _shouldSprint = PlayerHelper.IsInputPressed(context.ReadValue<float>());
    }

    private void OnReloadCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("ReloadCanceled");
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        _isAiming = PlayerHelper.IsInputPressed(context.ReadValue<float>());
    }
    private void OnSingleFirePerformed(InputAction.CallbackContext context)
    {
        WeaponConfiguration activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        ParticleSystem activeShootSystem = WeaponInventory.Instance.ActiveWeaponShootSystem;

        if (activeWeaponConfig.ShootConfig.IsSingleFire)
        {
            activeWeaponConfig.FireWeapon(activeShootSystem, PlayerHelper.IsInputPressed(context.ReadValue<float>()));
            if (activeWeaponConfig.AmmoConfig.ClipAmmo > 0 && PlayerHelper.IsInputPressed(context.ReadValue<float>()))
            {
                OnFire?.Invoke();
            }
        }
    }

    private void OnAutoFirePerformed(InputAction.CallbackContext context)
    {
        WeaponConfiguration activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        ParticleSystem activeShootSystem = WeaponInventory.Instance.ActiveWeaponShootSystem;

        if (activeWeaponConfig.ShootConfig.IsAutomaticFire)
        {
            if (_autoFireCoroutine != null)
            {
                StopCoroutine(_autoFireCoroutine);
            }

            if (_autoFireCoroutine == null) //double check just to avoid unnessary restarts
            {
                _autoFireCoroutine = StartCoroutine(AutoFire(activeWeaponConfig, activeShootSystem));
            }
        }
    }

    private IEnumerator AutoFire(WeaponConfiguration activeWeaponConfig, ParticleSystem shootSystem)
    {
        // Continue to fire the weapon while the automatic fire button is held down
        while (true)
        {
            if (activeWeaponConfig.AmmoConfig.ClipAmmo > 0)
            {
                OnFire?.Invoke();
            }
            activeWeaponConfig.FireWeapon(shootSystem, true);
            yield return null;
            yield return new WaitForSeconds(activeWeaponConfig.ShootConfig.FireRate);
        }
    }

    private void OnAutoFireCanceled(InputAction.CallbackContext context)
    {
        WeaponConfiguration activeWeaponConfig = WeaponInventory.Instance.ActiveWeaponConfig;
        ParticleSystem activeShootSystem = WeaponInventory.Instance.ActiveWeaponShootSystem;

        if (activeWeaponConfig.ShootConfig.IsAutomaticFire)
        {
            if (_autoFireCoroutine != null)
            {
                StopCoroutine(_autoFireCoroutine);
                _autoFireCoroutine = null;
            }

            activeWeaponConfig.FireWeapon(activeShootSystem, false);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        WeaponBox.Instance.ShouldInteract = true;
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        WeaponBox.Instance.ShouldInteract = false;
    }

    private void ApplyMovement()
    {
        float currentSpeed = _shouldSprint ? _moveSpeed * _sprintMultiplier : _moveSpeed;
        Vector3 movement = PlayerHelper.CalculateMovement(_moveInput);

        if (movement.magnitude > 0)
        {
            Move(movement, currentSpeed);
        }
        else
        {
            ApplyDrag(_drag);
        }

        LookAtDirection(movement);
        UpdateAnimation(movement);
        KeepWithinUnitCircle(_mapCenter.position, _mapRadius);
    }

    private void ApplyRotation()
    {
        if (Mathf.Abs(_angle) > 0)
        {
            RotateY(Mathf.Sign(_angle) * _sensitivity, _moveInput);
        }
    }

    //Separate reloading logic

    



}
