using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;
    private EnemyState _currentState;
    private Health _health;
    [SerializeField] private GameObject _moneyPrefab;
    [SerializeField] private ENEMYTYPE _type;
    public UnityEvent OnEnemyDefeated;

    private void Start()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        _health = this.GetComponent<Health>();
        _player = GameEnvironment.Instance.Player.transform;
        _currentState = new Idle(this.gameObject, _health, _agent, _animator, _player , _moneyPrefab, _type);
    }

    private void Update()
    {
        EnemyState newState = _currentState.ProcessState();
        if (_currentState is not Dead && newState is Dead)
        {
            OnEnemyDefeated?.Invoke();
        }
        _currentState = newState;
    }
}
