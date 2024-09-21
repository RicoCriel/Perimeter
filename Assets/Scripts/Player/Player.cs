using UnityEngine;

public class Player
{
    protected float moveSpeed;
    protected const float RotationSpeed = 720f;
    protected const float SmoothTime = 0.1f;
    protected CharacterController characterController;
    private Vector3 _currentVelocity = Vector3.zero;
    private float _verticalVelocity;
    private const float _gravity = -9.81f;

    public Player(float moveSpeed, CharacterController characterController)
    {
        this.moveSpeed = moveSpeed;
        this.characterController = characterController;
    }

    public void Move(Vector3 direction, float speed, float deltaTime)
    {
        Vector3 horizontalMovement = direction * speed * deltaTime;

        if (characterController.isGrounded)
        {
            _verticalVelocity = 0f; 
        }
        else
        {
            _verticalVelocity += _gravity * deltaTime; 
        }

        // Include vertical velocity in the movement vector
        Vector3 movement = horizontalMovement + new Vector3(0, _verticalVelocity, 0);

        _currentVelocity = movement;
        characterController.Move(movement);
    }

    public void ApplyDrag(float drag, float deltaTime)
    {
        // Apply drag to the current velocity
        _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, drag * deltaTime);
        characterController.Move(_currentVelocity * deltaTime);
    }

    public void Rotate(float mouseX)
    {
        float rotationAmount = mouseX * RotationSpeed * Time.deltaTime;

        // Smooth rotation interpolation
        float currentAngle = characterController.transform.rotation.eulerAngles.y;
        float targetAngle = currentAngle + rotationAmount;
        float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, SmoothTime);

        // Apply rotation
        characterController.transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
    }

    public void KeepWithinUnitCircle(Vector3 center, float mapRadius)
    {
        Vector3 offset = characterController.transform.position - center;
        float distance = offset.magnitude;

        // If the distance is greater than the radius
        if (distance > mapRadius)
        {
            // Normalize the offset to get the direction
            Vector3 direction = offset.normalized;

            // Calculate the target position at the edge of the circle
            Vector3 targetPos = center + direction * mapRadius;

            // Update the character's position to stay within bounds
            characterController.transform.position = targetPos;
        }
    }

    public void UpdateAnimation(Animator animator, Vector3 input)
    {
        animator.SetFloat("Speed", input.magnitude);
    }

    public Vector3 Position
    {
        get => characterController.transform.position;
    }

    public Quaternion Rotation
    {
        get => characterController.transform.rotation;
    }
}
