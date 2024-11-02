using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pursue : EnemyState
{
    public Pursue(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player) : base(npc, health, agent, anim, player)
    {
        Name = STATE.PURSUE;
        _agent.speed = 3f;
        _agent.isStopped = false;   
    }

    public override void Enter()
    {
        _anim.SetFloat("MoveSpeed", 1);
        base.Enter();
    }

    public override void Update()
    {
        _agent.SetDestination(_player.position);
        _npc.transform.LookAt(_player.position);

        if(_agent.hasPath)
        { 
            if(CanAttackPlayer())
            {
                _nextState = new Attack(_npc, _health, _agent, _anim, _player);
                _stage = EVENT.EXIT;
            }
            else if(!CanSeePlayer())
            { 
                _nextState = new Roam(_npc, _health, _agent, _anim, _player);
                _stage = EVENT.EXIT;
            }

            if (!IsAlive())
            {
                _nextState = new Dead(_npc, _health, _agent, _anim, _player);
                _stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        _agent.speed = 0;
        _anim.SetFloat("MoveSpeed", 0);
        base.Exit();
    }
}
