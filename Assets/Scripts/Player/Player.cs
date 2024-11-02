using UnityEngine;

public class Player: MonoBehaviour 
{
    protected const float _rotationSpeed = 360f;
    protected CharacterController _characterController;
    protected Animator _animator;

    private Vector3 _currentVelocity = Vector3.zero;
    private float _verticalVelocity;
    protected bool _isAiming;
    private const float _gravity = -9.81f;

    protected void GetComponents()
    {
        _characterController = this.GetComponent<CharacterController>();
        _animator = this.GetComponent<Animator>();
    }

    protected void Move(Vector3 direction, float speed)
    {
        if(_isAiming)
        {
            return;
        }

        Vector3 horizontalMovement = direction * speed * Time.deltaTime;

        if (_characterController.isGrounded)
        {
            _verticalVelocity = 0f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        Vector3 movement = horizontalMovement + new Vector3(0, _verticalVelocity, 0);

        _currentVelocity = movement;
        _characterController.Move(movement);
    }

    protected void ApplyDrag(float drag)
    {
        _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, drag * Time.deltaTime);
        _characterController.Move(_currentVelocity * Time.deltaTime);
    }

    protected void LookAtDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f && !_isAiming)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _characterController.transform.rotation = Quaternion.RotateTowards(
                _characterController.transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    protected void RotateY(float input, Vector2 moveInput)
    {
        if(_isAiming || moveInput.magnitude <= 0)
        {
            float currentAngle = _characterController.transform.rotation.eulerAngles.y;
            float newAngle = input * _rotationSpeed * Time.deltaTime;
            newAngle = currentAngle + newAngle;
            float targetAngle = Mathf.Lerp(currentAngle, newAngle, _rotationSpeed * Time.deltaTime);
            _characterController.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
    }

    protected void KeepWithinUnitCircle(Vector3 center, float mapRadius)
    {
        Vector3 offset = _characterController.transform.position - center;
        float distance = offset.magnitude;

        if (distance > mapRadius)
        {
            Vector3 direction = offset.normalized;
            Vector3 targetPos = center + direction * mapRadius;
            _characterController.transform.position = targetPos;
        }
    }

    protected void UpdateAnimation(Vector3 input)
    {
        //refactor
        if(_isAiming)
        {
            _animator.SetFloat("Speed", 0);
        }
        else
        {
            _animator.SetFloat("Speed", input.magnitude);
        }
    }

    public Vector3 Position
    {
        get => _characterController.transform.position;
    }

    public Quaternion Rotation
    {
        get => _characterController.transform.rotation;
    }
}
