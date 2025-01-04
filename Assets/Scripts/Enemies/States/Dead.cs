using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Dead : EnemyState
{
    private AudioSource _dyingAudioSource;
    private Collider _collider;
    private float _deadTime;
    private float _disableTime = 2f;
    private int _moneyAmount = Random.Range(1, 3);

    public Dead(GameObject npc, Health health, NavMeshAgent agent, Animator anim, Transform player, GameObject money)
        : base(npc, health, agent, anim, player, money)
    {
        Name = STATE.DEAD;
        _dyingAudioSource = npc.GetComponent<AudioSource>();
        _collider = npc.GetComponent<Collider>();   
        _agent.speed = 0f;
        _agent.isStopped = true;
    }

    public override void Enter()
    {
        //trigger ragdoll
        _collider.enabled = false;  
        SpawnMoney();
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

    private void SpawnMoney()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-1.5f, 1.5f), 
            Random.Range(0, 1.5f),                        
            Random.Range(-1.5f, 1.5f) 
        );

        Vector3 spawnPosition = _npc.transform.position + randomOffset;

        for(int i = 0; i < _moneyAmount; i++)
        {
            GameObject.Instantiate(_money, spawnPosition, Quaternion.identity);
        }
    }
}
