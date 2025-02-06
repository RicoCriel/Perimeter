using UnityEngine;
using DG.Tweening;

public class Loot : MonoBehaviour
{
    private Transform _playerTransform;
    private Tweener _tweener;

    private void OnEnable()
    {
        _playerTransform = FindObjectOfType<PlayerController>()?.transform;

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

        _tweener = transform.DOMove(targetPosition, 10f)
            .SetSpeedBased()
            .SetEase(Ease.OutQuad).OnComplete(() => SendLootReceivedMessage());
    }

    private void SendLootReceivedMessage()
    {
        Debug.Log("Player received" + this.name);
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerMoved -= MoveTowardsPlayer; 
    }
}
