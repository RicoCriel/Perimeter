using UnityEngine;
using DG.Tweening;

public class Loot : MonoBehaviour
{
    private Transform _playerTransform;
    private Tweener _tweener;
    private float _moveSpeed;

    private void OnEnable()
    {
        _playerTransform = FindObjectOfType<PlayerController>()?.transform;
        _moveSpeed = Random.Range(10f, 15f);

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
        //update ui
        this.gameObject.SetActive(false);
        //return to object pool
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerMoved -= MoveTowardsPlayer; 
    }
}
