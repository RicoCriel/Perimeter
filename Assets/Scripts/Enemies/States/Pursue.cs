using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pursue : EnemyState
{
    public Pursue(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player, GameObject money, ENEMYTYPE type)
        : base(npc, health, agent, anim, player, money, type)
    {
        Name = STATE.PURSUE;
        switch (type)
        {
            case ENEMYTYPE.REGULAR:
                _agent.speed = 3f;
                _agent.isStopped = false;
                break;
            case ENEMYTYPE.CRAWLER:
                _agent.speed = 5f;
                _agent.isStopped = false;
                break;
            case ENEMYTYPE.BIG:
                _agent.speed = 10f;
                _agent.isStopped = false;
                break;
            case ENEMYTYPE.BOSS:
                _agent.speed = 2f;
                _agent.isStopped = false;
                break;
            default:
                break;
        }
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
                _nextState = new Attack(_npc, _health, _agent, _anim, _player, _money, _type);
                _stage = EVENT.EXIT;
            }
            else if(!CanSeePlayer())
            { 
                _nextState = new Roam(_npc, _health, _agent, _anim, _player, _money, _type);
                _stage = EVENT.EXIT;
            }

            if (!IsAlive())
            {
                _nextState = new Dead(_npc, _health, _agent, _anim, _player, _money, _type);
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
