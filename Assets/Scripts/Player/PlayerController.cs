using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Singleton")]
    public static PlayerController Instance;

    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintMultiplier;
    [SerializeField] private float _drag;

    [Header("Map Bounds")]
    [SerializeField] private Transform _mapCenter;
    [SerializeField] private float _mapRadius;

    [Header("Mouse Input Settings")]
    [Range(0f, 15f)]
    [SerializeField] private float _sensitivity;
    [SerializeField] private bool _shouldInvertMouse;

    private Player _player;
    private CharacterController _characterController;
    private Animator _animator;

    public TextMeshProUGUI ScoreText;

    private void Awake()
    {
        Instance = this;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _player = new Player(_moveSpeed, _characterController);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        RotatePlayer();
        MovePlayer();

        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ScoreManager.Instance.IncreaseScore(5000,ScoreText);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            ScoreManager.Instance.DecreaseScore(5000,ScoreText);
        }
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? _moveSpeed * _sprintMultiplier : _moveSpeed;

        if (movement.magnitude > 0)
        {
            _player.Move(movement, currentSpeed, Time.deltaTime);
        }
        else
        {
            _player.ApplyDrag(_drag, Time.deltaTime);
        }

        _player.UpdateAnimation(_animator, movement);
        _player.KeepWithinUnitCircle(_mapCenter.position, _mapRadius);
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X");
        if(_shouldInvertMouse)
        {
            mouseX = -mouseX;
        }
        _player.Rotate(mouseX * _sensitivity);
    }
}
