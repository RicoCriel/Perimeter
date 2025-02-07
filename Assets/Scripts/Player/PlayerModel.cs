using UnityEngine;

public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Falling,
    Aiming,
    Dead
}

public class PlayerModel
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float AimInput { get; private set; }
    public float MoveSpeed { get; private set; }
    public float RotationSpeed { get; private set; }
    public float SprintMultiplier { get; private set; }
    public float CurrentSpeed { get; private set; }
    public PlayerState CurrentState { get; private set; }
    public bool IsSprinting { get; private set; }

    public float Acceleration;
    public float Deceleration;

    public float VerticalVelocity;
    private const float _gravity = -9.81f;
    private float _gravityMultiplier = 3f;
    private float _fallTime;
    private const float _fallThreshold = 1f;

    public PlayerModel(float moveSpeed, float rotationSpeed, float sprintMultiplier, float acceleration, float deceleration)
    {
        MoveSpeed = moveSpeed;
        RotationSpeed = rotationSpeed;
        SprintMultiplier = sprintMultiplier;
        Acceleration = acceleration;
        Deceleration = deceleration;
        CurrentSpeed = 0f;
        IsSprinting = false;
    }

    public void UpdateMoveInput(Vector2 input)
    {
        MoveInput = input;
    }

    public void UpdateLookInput(Vector2 lookInput)
    { 
        LookInput = lookInput;
    }

    public void UpdateAimInput(float input)
    {
        AimInput = input;
    }

    public void SetWalkingState(bool isWalking)
    {
        if(isWalking && CurrentState != PlayerState.Sprinting)
        {
            CurrentState = PlayerState.Walking;
        }
        else if(!isWalking)
        {
            CurrentState = PlayerState.Idle;
        }
    }

    public void SetSprintState(bool isSprinting)
    {
        if(isSprinting && MoveInput.sqrMagnitude > 0)
        {
            CurrentState = PlayerState.Sprinting;
        }
    }

    public void SetAimingState(bool isAiming)
    { 
        if(isAiming)
        {
            CurrentState = PlayerState.Aiming;
        }
    }

    public void SetDeadState(bool isDead)
    {
        if(isDead)
        {
            CurrentState = PlayerState.Dead;
        }
    }

    public void UpdateSpeed()
    {
        float targetSpeed = CurrentState == PlayerState.Sprinting ? MoveSpeed * SprintMultiplier : MoveSpeed;

        if (MoveInput.magnitude > 0)
        {
            //Fix acceleration and deceleration properly later

            //CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);
        }
        else
        {
            targetSpeed = 0;
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, Deceleration * Time.deltaTime);
        }

        //Debug.Log(CurrentSpeed);
    }


    public void ApplyGravity(CharacterController characterController)
    {
        if (characterController.isGrounded)
        {
            VerticalVelocity = 0;
            _fallTime = 0;
            SetFallingState(false);
        }
        else
        {
            VerticalVelocity += (_gravity * _gravityMultiplier) * Time.deltaTime;
            _fallTime += Time.deltaTime;

            if (_fallTime > _fallThreshold)
            {
                SetFallingState(true);
            }
        }
    }

    private void SetFallingState(bool isFalling)
    { 
        if(isFalling)
        { 
            CurrentState = PlayerState.Falling;
        }
    }

}

