using UnityEngine;

public class ChickenEnemy : Enemy
{
    public float FoxSpeed;
    public float FoxAcceleration;
    public float FoxTargetSpeed;

    private GameObject _targetChicken;

    protected override void Start()
    {
        base.Start();
        targetType = TargetType.Chicken;
    }

    protected override void SetRandomDestination()
    {
        _targetChicken = GameManager.Instance.GetRandomChicken();
        if (_targetChicken != null)
        {
            SetTargetDestination(_targetChicken.transform.position);
        }
        else
        {
            // Handle no chickens left (e.g., stop the enemy or make it wander)
            StopAgent(0);
            Debug.Log("No chickens left to target.");
        }
    }

    private void Update()
    {
        if (targetType == TargetType.Chicken)
        {
            SetRandomDestination();
            SetAgentSpeed(FoxSpeed, FoxTargetSpeed, FoxAcceleration);
            // Check if the agent has a valid path and is moving
            if (_targetChicken != null && _agent.hasPath && !_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                Destroy(_targetChicken);
                SetRandomDestination();
            }
        }
    }
}
