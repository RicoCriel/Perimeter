using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int _moneyAmount;
    [Range(10,1000f)]
    [SerializeField] private float _explosionForce;
    [Range(10, 1000f)]
    [SerializeField] private float _explosionRadius;
    [Range(0, 5f)]
    [SerializeField] private float _lifeTime;

    private Rigidbody _rb;
    private Coroutine _deactivateMyself;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb.IsSleeping())
        {
            _rb.WakeUp();
        }

        _rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);

        if (_deactivateMyself != null)
        {
            StopCoroutine(_deactivateMyself);
        }
        _deactivateMyself = StartCoroutine(Deactivate());

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ScoreManager.Instance.IncreaseScore(_moneyAmount);
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds (_lifeTime);
        gameObject.SetActive(false);
    }
    
}
