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
}
