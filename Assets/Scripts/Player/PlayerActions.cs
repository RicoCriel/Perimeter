using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerActions : Player
{
    [Header("Player Movement Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintMultiplier;
    [SerializeField] private float _drag;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;

    [Header("Player Aiming Settings")]
    [SerializeField] private Transform _aimTarget;
    [SerializeField] private Image _aimCrosshair;
    [SerializeField] private float _visionAngle;
    [SerializeField] private float _visionRange;
    [SerializeField] private float _aimHeight;
    [Range(1f, 20f)]
    [SerializeField] private float _aimSensitivity;
    [Range(0,60f)]
    [SerializeField] private float _crossHairSpeed;
    [Range(0,10f)]
    [SerializeField] private float _aimTargetSpeed;

    [Header("Reload Settings")]
    [SerializeField] private bool _shouldAutoReload;

    [Header("Map Settings")]
    [SerializeField] private Transform _mapCenter;
    [SerializeField] private float _mapRadius;

    public InputActionAsset PrimaryActions;
    private InputActionMap _gameplayActionMap;
    private InputAction _moveInputAction;
    private InputAction _lookInputAction;
    private InputAction _sprintInputAction;
    private InputAction _singleFireInputAction;
    private InputAction _automaticFireInputAction;
    private InputAction _interactInputAction;
    private InputAction _aimInputAction;
    private InputAction _reloadInputAction;

    private Vector2 _moveInput;
    private Vector2 _aimInput;

    private float _currentSpeed = 0f;
    private bool _shouldSprint;

    private Coroutine _autoFireCoroutine; 
    public UnityEvent OnFire;

    public override float VisionAngle => _visionAngle;
    public override float VisionRange => _visionRange;
    public override float AimHeight => _aimHeight;
    public override Image AimCrossHair => _aimCrosshair;
    public override Transform AimTarget => _aimTarget;
    public override float CrosshairSpeed => _crossHairSpeed;
    public override float AimTargetSpeed => _aimTargetSpeed;

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
        ControlAimingTarget(_aimInput, _aimTarget, _aimSensitivity);
        HandleAimTargetReset();
        AimAssist(_aimTarget, _aimInput, _aimSensitivity);
        UpdateCrosshairPosition();
    }

    private void FindInputActions()
    {
        _moveInputAction = _gameplayActionMap.FindAction("Move");
        _lookInputAction = _gameplayActionMap.FindAction("Look");
        _sprintInputAction = _gameplayActionMap.FindAction("Sprint");
        _singleFireInputAction = _gameplayActionMap.FindAction("Single Fire");
        _automaticFireInputAction = _gameplayActionMap.FindAction("Auto Fire");
        _interactInputAction = _gameplayActionMap.FindAction("Interact");
        _reloadInputAction = _gameplayActionMap.FindAction("Reload");
        _aimInputAction = _gameplayActionMap.FindAction("Aim");
    }

    private void EnableInputActions(bool isEnabled)
    {
        if (isEnabled)
        {
            _moveInputAction.Enable();
            _lookInputAction.Enable();
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
            _lookInputAction.Disable();
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
        _lookInputAction.performed += OnLookPerformed; 
        _singleFireInputAction.performed += OnSingleFirePerformed;
        _automaticFireInputAction.started += OnAutoFirePerformed;
        _interactInputAction.performed += OnInteractPerformed;
        _reloadInputAction.performed += OnReloadCanceled;
        _aimInputAction.performed += OnAimPerformed;

        _sprintInputAction.canceled += OnSprintPerformed;
        _moveInputAction.canceled += OnMoveCanceled;
        _lookInputAction.canceled += OnLookCanceled;
        _singleFireInputAction.canceled += OnSingleFirePerformed;
        _automaticFireInputAction.canceled += OnAutoFireCanceled;
        _interactInputAction.canceled += OnInteractCanceled;
        _reloadInputAction.canceled += OnReloadCanceled;
        _aimInputAction.canceled += OnAimPerformed;
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _aimInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        _aimInput = context.ReadValue<Vector2>().normalized;
    }

    private void UnsubscribeInputActionEvents()
    {
        _sprintInputAction.performed -= OnSprintPerformed;
        _moveInputAction.performed -= OnMovePerformed;
        _lookInputAction.performed -= OnLookPerformed;
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

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _shouldSprint = PlayerHelper.IsInputPressed(context.ReadValue<float>());
    }

    private void OnReloadCanceled(InputAction.CallbackContext context)
    {
        //Debug.Log("ReloadCanceled");
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
            activeWeaponConfig.FireWeapon(activeShootSystem, 
                PlayerHelper.IsInputPressed(context.ReadValue<float>()),
                _aimCrosshair , _aimTarget);

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
            activeWeaponConfig.FireWeapon(shootSystem, true, _aimCrosshair, _aimTarget);
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

            activeWeaponConfig.FireWeapon(activeShootSystem, false, _aimCrosshair, _aimTarget);
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
        float targetSpeed = _shouldSprint ? _moveSpeed * _sprintMultiplier : _moveSpeed;
        Vector3 movement = PlayerHelper.CalculateMovement(_moveInput);

        if (movement.magnitude > 0)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _acceleration * Time.deltaTime);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _deceleration * Time.deltaTime);
            _currentSpeed = Mathf.Clamp01(0);
        }

        Move(movement, _currentSpeed);

        LookAtDirection(movement);
        UpdateAnimation(movement);
        KeepWithinUnitCircle(_mapCenter.position, _mapRadius);
    }

}
