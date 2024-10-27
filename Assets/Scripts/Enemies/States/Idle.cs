using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : EnemyState
{
    public Idle(GameObject npc, NavMeshAgent agent, Animator anim, Transform player) : base(npc, agent, anim, player)
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
            _nextState = new Pursue(_npc, _agent, _anim, _player);
            _stage = EVENT.EXIT;
        }
        else if(Random.Range(0,100) < 25) //25% chance to roam around
        {
            _nextState = new Roam(_npc, _agent, _anim, _player);
            _stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        _anim.SetFloat("MoveSpeed", 0);
        base.Exit();
    }
}
