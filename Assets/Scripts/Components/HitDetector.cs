using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [SerializeField]
    private Health _health;
    [SerializeField]
    private int _hitDamage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && _health.CurrentHealth > 0)
        {
            other.GetComponent<Health>().DecreaseHealth();
            PostProcessManager.Instance.PlayVignette();
        }
    }
}
