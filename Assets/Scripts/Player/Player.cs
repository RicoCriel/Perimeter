using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player: MonoBehaviour 
{
    private const float _rotationSpeed = 360f;
    private const float _gravity = -9.81f;

    private Collider _currentLockedEnemyCollider;
    private bool _isAimTargetInitialized;

    protected CharacterController _characterController;
    protected Animator _animator;

    protected Vector3 _currentVelocity = Vector3.zero;
    private float _verticalVelocity;
    protected bool _isAiming;

    public virtual float VisionAngle { get; set; }
    public virtual float VisionRange { get; set; }
    public virtual float AimHeight { get; set; }
    public virtual Image AimCrossHair { get; set; }
    public virtual Transform AimTarget { get; set; }
    public virtual float AimTargetSpeed { get; set; }
    public virtual float CrosshairSpeed { get; set; }

    private Vector3 _originalAimTargetPosition;
    private float _aimInputTimer = 0f;
    private const float _aimResetDelay = 1f;

    protected void Start()
    {
        if (AimTarget != null)
        {
            _originalAimTargetPosition = AimTarget.localPosition;
        }
    }

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

        Vector3 movement = direction * speed * Time.deltaTime;

        if (_characterController.isGrounded)
        {
            _verticalVelocity = 0f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        movement = movement + new Vector3(0, _verticalVelocity, 0);

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

    protected void AimAssist(Transform aimTarget, Vector2 aimInput, float aimSensitivity)
    {
        if (!_isAiming) return;

        GameObject closestEnemy = null;
        float closestAngle = Mathf.Infinity;

        foreach (var enemy in FindObjectsOfType<AI>())
        {
            Vector3 directionToEnemy = (enemy.transform.position - Position).normalized;

            float dot = Vector3.Dot(_characterController.transform.forward, directionToEnemy);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle > VisionAngle)
            {
                continue;
            }

            float distanceToEnemy = Vector3.Distance(Position, enemy.transform.position);
            if (distanceToEnemy > VisionRange)
            {
                continue;
            }

            Ray ray = new Ray(Position + Vector3.up * AimHeight, enemy.transform.position - Position);
            if (Physics.Raycast(ray, out RaycastHit hit, VisionRange))
            {
                if (hit.collider.gameObject != enemy.gameObject)
                {
                    continue;
                }
            }

            if (angle < closestAngle)
            {
                closestEnemy = enemy.gameObject;
                closestAngle = angle;
            }
        }

        if (closestEnemy != null)
        {
            Collider enemyCollider = closestEnemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                float torsoOffset = enemyCollider.bounds.extents.y;
                Vector3 torsoPosition = closestEnemy.transform.position + Vector3.up * torsoOffset;
                Vector3 targetPos = new Vector3(torsoPosition.x, _characterController.transform.position.y, torsoPosition.z);
                _characterController.transform.LookAt(targetPos);

                EnemyTargetLockOn(aimInput, aimTarget, aimSensitivity, enemyCollider);
            }
        }
    }

    protected void ControlAimingTarget(Vector2 aimInput, Transform target, float aimSensitivity)
    {
        if (_isAiming) return;

        if (aimInput.sqrMagnitude > 0.01f)
        {
            _aimInputTimer = 0f; 
        }

        Vector3 newTargetPosition = target.localPosition + new Vector3(aimInput.x, aimInput.y, 0) * aimSensitivity * Time.deltaTime;
        target.localPosition = newTargetPosition;
    }

    protected void HandleAimTargetReset()
    {
        if (!_isAiming)
        {
            _aimInputTimer += Time.deltaTime;

            if (_aimInputTimer >= _aimResetDelay)
            {
                AimTarget.localPosition = Vector3.Lerp(
                    AimTarget.localPosition,
                    _originalAimTargetPosition,
                    Time.deltaTime * CrosshairSpeed
                );

                // If the target is close enough to the original position, snap it
                if (Vector3.Distance(AimTarget.localPosition, _originalAimTargetPosition) < 0.01f)
                {
                    AimTarget.localPosition = _originalAimTargetPosition;
                    _aimInputTimer = 0f; // Reset the timer after snapping
                }
            }
        }
        else
        {
            _aimInputTimer = 0f; // Reset the timer if aiming
        }
    }

    private void EnemyTargetLockOn(Vector2 aimInput, Transform aimTarget, float aimSensitivity, Collider enemyCollider)
    {
        if (_currentLockedEnemyCollider != enemyCollider)
        {
            _currentLockedEnemyCollider = enemyCollider;
            _isAimTargetInitialized = false;
        }

        Bounds enemyBounds = enemyCollider.bounds;
        Vector3 targetPosition = enemyBounds.center;

        Rigidbody enemyRigidbody = enemyCollider.GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            targetPosition += enemyRigidbody.velocity * Time.deltaTime;
        }

        aimTarget.position = Vector3.Lerp(aimTarget.position, targetPosition, Time.deltaTime * CrosshairSpeed);

        Vector3 aimingMovement = new Vector3(0, aimInput.y, aimInput.x) * aimSensitivity * Time.deltaTime;
        Vector3 newTargetPosition = aimTarget.position + aimingMovement;

        newTargetPosition.x = Mathf.Clamp(newTargetPosition.x, enemyBounds.min.x, enemyBounds.max.x);
        newTargetPosition.y = Mathf.Clamp(newTargetPosition.y, enemyBounds.min.y, enemyBounds.max.y);
        newTargetPosition.z = Mathf.Clamp(newTargetPosition.z, enemyBounds.min.z, enemyBounds.max.z);

        aimTarget.position = newTargetPosition;
    }

    protected void UpdateCrosshairPosition()
    {
        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(AimTarget.position);
        AimCrossHair.rectTransform.position = Vector3.Lerp(
            AimCrossHair.rectTransform.position,
            targetScreenPosition,
            Time.deltaTime * CrosshairSpeed
        );
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
