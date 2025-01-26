using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ENEMYTYPE
{
    CRAWLER, REGULAR, BIG, BOSS
}

public class EnemyState
{
    public enum STATE
    {
        IDLE, ROAM, PURSUE, ATTACK, DEAD
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE Name;
    protected EVENT _stage;
    protected GameObject _npc;
    protected Health _health;
    protected Animator _anim;
    protected Transform _player;
    protected EnemyState _nextState;
    protected NavMeshAgent _agent;
    protected GameObject _money;
    protected ENEMYTYPE _type;
    //Tweak these values
    private float _visionDistance = 30.0f;
    private float _visionAngle = 60.0f;
    private float _attackDistance = 3f;

    public EnemyState(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player, GameObject money, ENEMYTYPE type)
    {
        _npc = npc;
        _health = health;
        _agent = agent;
        _anim = anim;
        _stage = EVENT.ENTER;
        _player = player;
        _money = money;
        _type = type;
    }

    public EnemyState(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player, GameObject money)
    {
        _npc = npc;
        _health = health;
        _agent = agent;
        _anim = anim;
        _player = player;
        _money = money;
    }

    public virtual void Enter() { _stage = EVENT.UPDATE; }
    public virtual void Update() { _stage = EVENT.UPDATE; }
    public virtual void Exit() { _stage = EVENT.EXIT; }

    public EnemyState ProcessState()
    {
        if (_stage == EVENT.ENTER) Enter();
        if (_stage == EVENT.UPDATE) Update();
        if (_stage == EVENT.EXIT)
        {
            Exit();
            return _nextState;
        }
        return this;
    }

    protected bool CanSeePlayer()
    {
        Vector3 direction = _player.position - _npc.transform.position;
        float angle = Vector3.Angle(direction, _npc.transform.forward);

        if (direction.magnitude < _visionDistance && angle < _visionAngle)
        {
            return true;
        }
        return false;
    }

    protected bool CanAttackPlayer()
    {
        Vector3 direction = _player.position - _npc.transform.position;
        if (direction.magnitude < _attackDistance)
        {
            return true;
        }
        return false;
    }

    protected bool IsAlive()
    {
        return _health.CurrentHealth > 0;
    }
}
