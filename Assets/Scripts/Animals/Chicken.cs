using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AnimalState
{
    Normal,
    Scared,
}

public class Chicken : MonoBehaviour
{
    private Vector3 _center = Vector3.zero; 
    private float _mapRadius = 19.0f;
    private float _newDestinationInterval;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _timer;
    private AnimalState _currentState;
    private Dictionary<AnimalState, float> _stateSpeedMappings = new Dictionary<AnimalState, float>
    {
        { AnimalState.Normal, 1f },
        { AnimalState.Scared, 3f }
    };


    private void Start()
    {
        GameManager.Instance.AddChicken(gameObject);
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _newDestinationInterval = Random.Range(10f, 15f);
        SetRandomDestination(_center, _mapRadius);
        _currentState = AnimalState.Normal;
    }

    private void Update()
    {
        if (_stateSpeedMappings.TryGetValue(_currentState, out float speed))
        {
            _agent.speed = speed;
            _animator.SetFloat("Speed", speed);
        }

        _timer += Time.deltaTime;
        if (_timer >= _newDestinationInterval || _agent.remainingDistance < 0.5f)
        {
            SetRandomDestination(_center, _mapRadius);
            _timer = 0;
        }
    }

    private void SetRandomDestination(Vector3 center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        Vector3 destination = new Vector3(randomPoint.x, center.y, randomPoint.y) + center;
        _agent.SetDestination(destination);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _currentState = AnimalState.Scared;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _currentState = AnimalState.Normal;
        }
    }
}
