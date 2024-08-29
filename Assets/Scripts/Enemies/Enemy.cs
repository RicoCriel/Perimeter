using UnityEngine;
using UnityEngine.AI;

public enum TargetType
{
    Chicken,
    Player
}

public abstract class Enemy : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected Animator _animator;
    public TargetType targetType;

    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();    
        _agent = GetComponent<NavMeshAgent>();
    }

    protected void SetTargetDestination(Vector3 targetPosition)
    {
        _agent.SetDestination(targetPosition);
    }

    protected void StopAgent(float animSpeed)
    {
        _agent.ResetPath();
        //animator.SetFloat("Speed", animSpeed);
    }

    public void SetAgentSpeed(float startSpeed, float targetSpeed, float speedIncrement)
    {
        if (_agent.speed < targetSpeed)
        {
            _agent.speed += speedIncrement * Time.deltaTime;
            _agent.speed = Mathf.Min(_agent.speed, targetSpeed); 
            //animator.SetFloat("Speed", agent.speed);
        }
    }

    protected abstract void SetRandomDestination();
}

