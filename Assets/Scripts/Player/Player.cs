using UnityEngine;

public class Player
{
    protected float moveSpeed;
    protected const float rotationSpeed = 720f;
    protected const float smoothTime = 0.1f;
    protected CharacterController characterController;
    private Vector3 _currentVelocity = Vector3.zero;

    public Player(float moveSpeed, CharacterController characterController)
    {
        this.moveSpeed = moveSpeed;
        this.characterController = characterController;
    }

    public void Move(Vector3 direction, float speed, float deltaTime)
    {
        Vector3 movement = direction * speed * deltaTime;
        _currentVelocity = movement; // Update current velocity based on movement
        characterController.Move(movement);
    }

    public void ApplyDrag(float drag, float deltaTime)
    {
        // Apply drag to the current velocity
        _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, drag * deltaTime);
        characterController.Move(_currentVelocity * deltaTime);
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        Vector3 directionToFace = targetPosition - characterController.transform.position;
        directionToFace.y = 0;

        if (directionToFace.magnitude > 0.8f)
        {
            float targetAngle = Mathf.Atan2(directionToFace.x, directionToFace.z) * Mathf.Rad2Deg;
            float currentAngle = characterController.transform.rotation.eulerAngles.y;
            float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            characterController.transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
        }
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
