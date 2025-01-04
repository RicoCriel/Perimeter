using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : EnemyState
{
    public Idle(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player, GameObject money)
        : base(npc, health, agent, anim, player, money)
    {
        Name = STATE.IDLE;
    }

    public override void Enter()
    {
        _anim.SetFloat("MoveSpeed", 0);
        base.Enter();
    }

    public override void Update()
    {
        if(CanSeePlayer())
        { 
            _nextState = new Pursue(_npc, _health, _agent, _anim, _player, _money);
            _stage = EVENT.EXIT;
        }
        else if(Random.Range(0,100) < 25) //25% chance to roam around
        {
            _nextState = new Roam(_npc, _health, _agent, _anim, _player, _money);
            _stage = EVENT.EXIT;
        }

        if(!IsAlive())
        { 
            _nextState = new Dead(_npc, _health, _agent, _anim, _player, _money);
            _stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        _anim.SetFloat("MoveSpeed", 0);
        base.Exit();
    }
}
