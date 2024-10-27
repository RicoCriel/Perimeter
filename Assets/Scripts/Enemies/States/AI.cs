using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;
    private EnemyState _currentState;

    private void Start()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        _player = GameEnvironment.Instance.Player.transform;
        _currentState = new Idle(this.gameObject, _agent, _animator, _player);
    }

    private void Update()
    {
        _currentState = _currentState.ProcessState();
    }
}
