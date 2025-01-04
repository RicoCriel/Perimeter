using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack : EnemyState
{
    private AudioSource _attackAudioSource; 

    public Attack(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player, GameObject money)
        : base(npc, health, agent, anim, player, money)
    {
        Name = STATE.ATTACK;
        _attackAudioSource = npc.GetComponent<AudioSource>();
        _agent.speed = 0;
        _agent.isStopped = true;
    }

    public override void Enter()
    {
        _attackAudioSource.Play();
        base.Enter();
    }

    public override void Update()
    {
        _anim.SetTrigger("Attack");

        if (!IsAlive())
        {
            _nextState = new Dead(_npc, _health, _agent, _anim, _player, _money);
            _stage = EVENT.EXIT;
        }

        if (!CanAttackPlayer())
        {
            _nextState = new Idle(_npc, _health, _agent, _anim, _player, _money);
            _stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        _anim.ResetTrigger("Attack");
        _attackAudioSource.Stop();
        base.Exit();
    }
}
