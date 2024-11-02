using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Roam : EnemyState
{
    public Roam(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player) : base(npc, health, agent, anim, player)
    {
        Name = STATE.ROAM;
        _agent.speed = 2f;
        _agent.isStopped = false;
    }

    public override void Enter()
    {
        _anim.SetFloat("MoveSpeed", 1);
        //Get random point in the map and go towards it
        _agent.SetDestination(GameEnvironment.Instance.GetRandomWanderPoint(20f));
        base.Enter();
    }

    public override void Update()
    {
        if(_agent.remainingDistance < 1)
        {
            _agent.SetDestination(GameEnvironment.Instance.GetRandomWanderPoint(20f));
        }

        if(CanSeePlayer())
        { 
            _nextState = new Pursue(_npc, _health, _agent, _anim, _player);
            _stage = EVENT.EXIT;
        }

        if (!IsAlive())
        {
            _nextState = new Dead(_npc, _health, _agent, _anim, _player);
            _stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        _anim.SetFloat("MoveSpeed", 0);
        base.Exit();
    }
}
