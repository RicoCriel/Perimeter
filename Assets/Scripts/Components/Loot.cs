using UnityEngine;
using DG.Tweening;
using System;

public class Loot : MonoBehaviour
{
    private Transform _playerTransform;
    private Tweener _tweener;
    private float _moveSpeed;

    [SerializeField] private string _lootType;
    private int _lootAmount;
    public static event Action<string> OnLootCollected; //Event to notify UI


    private void OnEnable()
    {
        _playerTransform = FindObjectOfType<PlayerController>()?.transform;
        _moveSpeed = UnityEngine.Random.Range(10f, 15f);
        _lootAmount = UnityEngine.Random.Range(1, 8);

        if (_playerTransform != null)
        {
            MoveTowardsPlayer(_playerTransform.position);
            PlayerController.OnPlayerMoved += MoveTowardsPlayer; 
        }
    }

    private void MoveTowardsPlayer(Vector3 targetPosition)
    {
        if (_tweener != null && _tweener.IsActive())
            _tweener.Kill(); 

        _tweener = transform.DOMove(targetPosition, _moveSpeed)
            .SetSpeedBased()
            .SetEase(Ease.OutQuad).OnComplete(() => SendLootReceivedMessage());
    }

    private void SendLootReceivedMessage()
    {
        OnLootCollected?.Invoke($"{_lootAmount} {_lootType}");
        this.gameObject.SetActive(false);
        //return to object pool
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerMoved -= MoveTowardsPlayer; 
    }
}
