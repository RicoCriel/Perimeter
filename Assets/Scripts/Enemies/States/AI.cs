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
    private Health _health;
    [SerializeField] private GameObject _moneyPrefab;

    private void Start()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        _health = this.GetComponent<Health>();
        _player = GameEnvironment.Instance.Player.transform;
        _currentState = new Idle(this.gameObject, _health, _agent, _animator, _player , _moneyPrefab);
    }

    private void Update()
    {
        _currentState = _currentState.ProcessState();
    }
}
