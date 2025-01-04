using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingSystem : MonoBehaviour
{
    public float VisionAngle;
    public float VisionRange;
    public float AimHeight;
    public Transform AimTarget;
    public float AimTargetSpeed;
    public float CrosshairSpeed;

    [SerializeField] private GameObject _targetVisual;
    [SerializeField] private float _aimSensitivity;

    private Vector3 _originalAimTargetPosition;
    private float _lookInputTimer = 0f;
    private const float _lookResetDelay = 1f;

    private readonly AimSubject _aimSubject = new AimSubject();

    private void Start()
    {
        if (AimTarget != null)
        {
            _originalAimTargetPosition = AimTarget.localPosition;
        }
    }

    public void RegisterObserver(IAimObserver observer)
    {
        _aimSubject.RegisterObserver(observer);
    }

    public void UnregisterObserver(IAimObserver observer)
    {
        _aimSubject.UnregisterObserver(observer);
    }

    public void Aim(Vector2 lookInput, float aimInput, PlayerState currentState)
    {
        if (currentState != PlayerState.Aiming || !PlayerHelper.IsInputPressed(aimInput))
        {
            _targetVisual.SetActive(false);
            _aimSubject.NotifyAimStopped();
            return;
        }

        GameObject closestEnemy = GetClosestVisibleEnemy();
        if (closestEnemy == null)
        {
            _targetVisual.SetActive(false);
            _aimSubject.NotifyAimStopped();
            return;
        }

        UpdateTargetVisual(_targetVisual, closestEnemy);
        LockOntoTarget(lookInput, closestEnemy.GetComponent<Collider>(), _aimSensitivity);
        _aimSubject.NotifyAimUpdated(AimTarget.position);
    }

    private GameObject GetClosestVisibleEnemy()
    {
        GameObject closestEnemy = null;
        float closestAngle = Mathf.Infinity;

        foreach (var enemy in FindObjectsOfType<AI>())
        {
            if (!IsEnemyInVision(enemy, out float angle)) continue;
            if (angle < closestAngle)
            {
                closestEnemy = enemy.gameObject;
                closestAngle = angle;
            }
        }
        return closestEnemy;
    }

    private bool IsEnemyInVision(AI enemy, out float angle)
    {
        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        angle = Mathf.Acos(Vector3.Dot(transform.forward, directionToEnemy)) * Mathf.Rad2Deg;

        if (angle > VisionAngle) return false;
        if (Vector3.Distance(transform.position, enemy.transform.position) > VisionRange) return false;

        if (!HasLineOfSight(enemy)) return false;

        return true;
    }

    private bool HasLineOfSight(AI enemy)
    {
        Ray ray = new Ray(transform.position + Vector3.up * AimHeight, enemy.transform.position - transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, VisionRange))
        {
            return hit.collider.gameObject == enemy.gameObject;
        }
        return false;
    }

    private void UpdateTargetVisual(GameObject targetVisual, GameObject targetEnemy)
    {
        targetVisual.SetActive(true);
        targetVisual.transform.position = targetEnemy.transform.position;
        _aimSubject.NotifyAimStarted();
    }

    private void LockOntoTarget(Vector2 lookInput, Collider targetCollider, float aimSensitivity)
    {
        Bounds bounds = targetCollider.bounds;
        Vector3 targetPosition = bounds.center;

        AimTarget.position = Vector3.Lerp(AimTarget.position, targetPosition, Time.deltaTime * CrosshairSpeed);

        Vector3 aimAdjustment = new Vector3(0, lookInput.y, lookInput.x) * aimSensitivity * Time.deltaTime;
        AimTarget.position += aimAdjustment;

        ClampAimToBounds(AimTarget.position, bounds);
    }

    private void ClampAimToBounds(Vector3 aimPosition, Bounds bounds)
    {
        aimPosition.x = Mathf.Clamp(aimPosition.x, bounds.min.x, bounds.max.x);
        aimPosition.y = Mathf.Clamp(aimPosition.y, bounds.min.y, bounds.max.y);
        aimPosition.z = Mathf.Clamp(aimPosition.z, bounds.min.z, bounds.max.z);
    }

    public void HandleAimTargetReset(PlayerState currentState)
    {
        if (currentState != PlayerState.Aiming)
        {
            _lookInputTimer += Time.deltaTime;

            if (_lookInputTimer >= _lookResetDelay)
            {
                AimTarget.localPosition = Vector3.Lerp(
                    AimTarget.localPosition,
                    _originalAimTargetPosition,
                    Time.deltaTime * CrosshairSpeed
                );

                if (Vector3.Distance(AimTarget.localPosition, _originalAimTargetPosition) < 0.01f)
                {
                    AimTarget.localPosition = _originalAimTargetPosition;
                    _lookInputTimer = 0f; 
                }
            }
        }
        else
        {
            _lookInputTimer = 0f; 
        }
    }

    public void ControlAimingTarget(Vector2 lookInput, PlayerState currentState)
    {
        if (currentState == PlayerState.Aiming) return;

        if (lookInput.sqrMagnitude > 0.01f)
        {
            _lookInputTimer = 0f;
        }

        Vector3 newTargetPosition = AimTarget.localPosition + new Vector3(lookInput.x, lookInput.y, 0) * _aimSensitivity * Time.deltaTime;
        AimTarget.localPosition = newTargetPosition;
    }

}
