using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Dead : EnemyState
{
    private AudioSource _dyingAudioSource;
    private float _deadTime;
    private float _disableTime = 2f;

    public Dead(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player) : base(npc, health, agent, anim, player)
    {
        Name = STATE.DEAD;
        _dyingAudioSource = npc.GetComponent<AudioSource>();
        _agent.speed = 0f;
        _agent.isStopped = true;
    }

    public override void Enter()
    {
        //trigger ragdoll
        _dyingAudioSource.Play();
        _anim.SetTrigger("Dead");
        base.Enter();
    }

    public override void Update()
    {
        _deadTime += Time.deltaTime;
        if (_deadTime > _disableTime)
        { 
            GameObject.Destroy(_npc);
            _deadTime = 0f;
        }
        base.Update();
    }

    public override void Exit()
    { 
        if(_npc != null)
        {
            _dyingAudioSource.Stop();
            _anim.ResetTrigger("Dead");
        }
        _stage = EVENT.EXIT;
    }
}
