using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //This class is responsible for all the playerinput and player actions related logic
    [SerializeField] private PlayerView _view;
    [Header("Player Movement Setttings")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _sprintMultiplier;
    [SerializeField] private float _rotationSpeed;

    private PlayerModel _model;
    private CharacterController _characterController;
    //private AimingSystem _aimingSystem;
    //private WeaponSystem _weaponSystem;
    private Collider[] _colliders = new Collider[5];

    [SerializeField] private InputActionAsset _playerControls;
    private InputActionMap _gameplayActionMap;
    private InputAction _moveInputAction;
    private InputAction _sprintInputAction;
    private InputAction _interactInputAction;
    private InputAction _lookInputAction;
    private InputAction _aimInputAction;
    private InputAction _singleFireInputAction;
    private InputAction _automaticFireInputAction;

    public static event Action<Vector3> OnPlayerMoved;

    private void Awake()
    {
        _gameplayActionMap = _playerControls.FindActionMap("Gameplay");
        _view = GetComponent<PlayerView>();
        _characterController = GetComponent<CharacterController>();
        _model = new PlayerModel(_maxSpeed, _rotationSpeed, _sprintMultiplier);
        _model.VerticalVelocity = 0;
        //_aimingSystem = GetComponent<AimingSystem>();
        //_weaponSystem = GetComponent<WeaponSystem>();
        //var targetObserver = GetComponent<TargetObserver>();

        //_aimingSystem.RegisterObserver(targetObserver);
    }

    private void OnEnable()
    {
        _moveInputAction = _gameplayActionMap.FindAction("Move");
        _sprintInputAction = _gameplayActionMap.FindAction("Sprint");
        _interactInputAction = _gameplayActionMap.FindAction("Interact");
        _lookInputAction = _gameplayActionMap.FindAction("Look");
        _aimInputAction = _gameplayActionMap.FindAction("Aim");
        _singleFireInputAction = _gameplayActionMap.FindAction("Single Fire");
        _automaticFireInputAction = _gameplayActionMap.FindAction("Auto Fire");

        _moveInputAction.Enable();
        _sprintInputAction.Enable();
        _interactInputAction.Enable();
        _lookInputAction.Enable();
        _aimInputAction.Enable();
        _singleFireInputAction.Enable();
        _automaticFireInputAction.Enable();

        _sprintInputAction.performed += OnSprintPerformed;
        _sprintInputAction.canceled += OnSprintCanceled;
        _moveInputAction.performed += OnMovePerformed;
        _moveInputAction.canceled += OnMoveCanceled;
        _interactInputAction.performed += OnInteractPerformed;
        _interactInputAction.canceled += OnInteractCanceled;
        _aimInputAction.performed += OnAimPerformed;
        _aimInputAction.canceled += OnAimCanceled;
        _lookInputAction.performed += OnLookPerformed;
        _lookInputAction.canceled += OnLookCanceled;
        _singleFireInputAction.performed += OnSingleFirePerformed;
        _automaticFireInputAction.performed += OnAutoFirePerformed;
        _automaticFireInputAction.canceled += OnAutoFireCanceled;
    }

    private void OnDisable()
    {
        _sprintInputAction.performed -= OnSprintPerformed;
        _sprintInputAction.canceled -= OnSprintCanceled;
        _moveInputAction.performed -= OnMovePerformed;
        _moveInputAction.canceled -= OnMoveCanceled;
        _interactInputAction.canceled -= OnInteractPerformed;
        _interactInputAction.performed -= OnInteractCanceled;
        _aimInputAction.performed -= OnAimPerformed;
        _aimInputAction.canceled -= OnAimCanceled;
        _lookInputAction.performed -= OnLookPerformed;
        _lookInputAction.canceled -= OnLookCanceled;
        _singleFireInputAction.performed -= OnSingleFirePerformed;
        _automaticFireInputAction.performed -= OnAutoFirePerformed;
        _automaticFireInputAction.canceled -= OnAutoFireCanceled;

        _moveInputAction.Disable();
        _sprintInputAction.Disable();
        _lookInputAction.Disable();
        _aimInputAction.Disable();
        _singleFireInputAction.Disable();
        _automaticFireInputAction.Disable();
    }

    private void Update()
    {
        _model.UpdateSpeed();
        _model.ApplyGravity(_characterController);
        Vector3 horizontalMovement = PlayerHelper.CalculateMovement(_model.MoveInput) * _model.CurrentSpeed;
        Vector3 verticalMovement = Vector3.up * _model.VerticalVelocity;
        Vector3 movement = (horizontalMovement + verticalMovement) * Time.deltaTime;
        _characterController.Move(movement);

        //_aimingSystem.ControlAimingTarget(_model.LookInput, _model.CurrentState);
        //_aimingSystem.Aim(_model.LookInput, _model.AimInput, _model.CurrentState);
        //_aimingSystem.HandleAimTargetReset(_model.CurrentState);
        //_weaponSystem.UpdateCrosshairPosition();

        _view.UpdateAnimation(_model.MoveInput);
        Vector3 movementDirection = horizontalMovement.normalized;
        if (movementDirection != Vector3.zero)
        {
            _view.LookAtDirection(movementDirection, _characterController, _rotationSpeed, _model);
        }

        if (OnPlayerMoved != null)
        {
            OnPlayerMoved.Invoke(transform.position);
        }

        _view.KeepWithinUnitCircle(_view.MapCenter.position, _view.MapRadius);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _model.UpdateMoveInput(context.ReadValue<Vector2>());
        _model.SetWalkingState(PlayerHelper.IsInputPressed(_model.MoveInput.sqrMagnitude));
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _model.UpdateMoveInput(Vector2.zero);
        _model.SetWalkingState(false);
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _model.SetSprintState(true);
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _model.SetSprintState(false);
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    { 
        _model.UpdateAimInput(context.ReadValue<float>());
        _model.SetAimingState(true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        _model.UpdateAimInput(0);
        _model.SetAimingState(false);

    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    { 
        _model.UpdateLookInput(context.ReadValue<Vector2>().normalized);
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _model.UpdateLookInput(Vector2.zero);
    }

    private void OnSingleFirePerformed(InputAction.CallbackContext context)
    {
        //_weaponSystem.SingleFire(context.ReadValue<float>(),_model.CurrentState);
    }

    private void OnAutoFirePerformed(InputAction.CallbackContext context)
    {
        //_weaponSystem.AutoFire(true, _model.CurrentState);
    }

    private void OnAutoFireCanceled(InputAction.CallbackContext context)
    {
        //_weaponSystem.AutoFire(false, _model.CurrentState);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Vector3 position = transform.position; // Player position
        int hitCount = Physics.OverlapSphereNonAlloc(position, _view.InteractRange, _colliders, ~0, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            if (_colliders[i].TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
                break; // Only interact with the first valid object
            }
        }
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        //WeaponBox.Instance.ShouldInteract = false;
    }
}
