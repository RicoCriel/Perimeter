using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState
{
    public enum STATE
    {
        IDLE, ROAM, PURSUE, ATTACK
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE Name;
    protected EVENT _stage;
    protected GameObject _npc;
    protected Animator _anim;
    protected Transform _player;
    protected EnemyState _nextState;
    protected NavMeshAgent _agent;
    //Tweak these values
    private float _visionDistance = 30.0f;
    private float _visionAngle = 60.0f;
    private float _attackDistance = 3f;

    public EnemyState(GameObject npc, NavMeshAgent agent, Animator anim, Transform player)
    {
        _npc = npc;
        _agent = agent;
        _anim = anim;
        _stage = EVENT.ENTER;
        _player = player;
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

    public bool CanSeePlayer()
    {
        Vector3 direction = _player.position - _npc.transform.position;
        float angle = Vector3.Angle(direction, _npc.transform.forward);

        if (direction.magnitude < _visionDistance && angle < _visionAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = _player.position - _npc.transform.position;
        if (direction.magnitude < _attackDistance)
        {
            return true;
        }
        return false;
    }
}
