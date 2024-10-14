using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : Player
{
    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintMultiplier;
    [SerializeField] private float _drag;

    [Header("Mouse Input Settings")]
    [Range(1f, 10f)]
    [SerializeField] private float _sensitivity;
    [SerializeField] private bool _shouldInvertX;

    [Header("Map Settings")]
    [SerializeField] private Transform _mapCenter;
    [SerializeField] private float _mapRadius;

    public InputActionAsset PrimaryActions;
    private InputActionMap _gameplayActionMap;
    private InputAction _rotateInputAction;
    private InputAction _moveInputAction;
    private InputAction _sprintInputAction;

    private Vector2 _input;
    private float _angle;
    private bool _shouldSprint;

    private void Awake()
    {
        Initialize();
        _gameplayActionMap = PrimaryActions.FindActionMap("Gameplay");
        _rotateInputAction = _gameplayActionMap.FindAction("Rotate");
        _moveInputAction = _gameplayActionMap.FindAction("Move");
        _sprintInputAction = _gameplayActionMap.FindAction("Sprint");

        _rotateInputAction.performed += GetRotateInput;
        _sprintInputAction.performed += GetSprintInput;
        _moveInputAction.performed += GetMoveInput;

        _rotateInputAction.canceled += GetRotateInput;
        _sprintInputAction.canceled += GetSprintInput;
        _moveInputAction.canceled += GetMoveInput;
    }

    private void OnEnable()
    {
        _rotateInputAction.Enable();
        _moveInputAction.Enable();
        _sprintInputAction.Enable();
    }

    private void OnDisable()
    {
        _rotateInputAction.Disable();
        _moveInputAction.Disable();
        _sprintInputAction.Disable();
    }

    private void GetMoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    private void GetRotateInput(InputAction.CallbackContext context)
    {
        _angle = context.ReadValue<float>();
    }

    private void GetSprintInput(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() > 0.5f)
        {
            _shouldSprint = true;
        }
        else
        {
            _shouldSprint = false;
        }
    }

    private void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        float currentSpeed = _shouldSprint ? _moveSpeed * _sprintMultiplier : _moveSpeed;
        Vector3 movement = new Vector3(_input.x, 0, _input.y);
        movement = -movement.ConvertToIsoCoords();

        if (movement.magnitude > 0)
        {
            Move(movement, currentSpeed);
            LookAtDirection(movement);
        }
        else
        {
            ApplyDrag(_drag);
        }

        KeepWithinUnitCircle(_mapCenter.position, _mapRadius);
        UpdateAnimation(movement);
    }

    private void RotatePlayer()
    {
        if (Mathf.Abs(_angle) > 0)
        {
            if (_shouldInvertX)
            {
                _angle = -_angle;
            }
            Rotate(Mathf.Sign(_angle) * _sensitivity);
        }
    }


}
