using UnityEngine;

public class Crate : Interactable
{
    [SerializeField] private InteractView _view;
    [SerializeField] private GameObject [] _componentPrefabs;
    [SerializeField] private Transform _lootSpawnPos;
    private bool _hasInteracted;
    private Animator _animator;

    private void Awake()
    {
        _view = GetComponentInParent<InteractView>();
        _animator = GetComponent<Animator>();
    } 

    public override void Interact()
    {
        if(!_hasInteracted)
        {
            _animator.SetTrigger("Open");
        }
        _view.ShouldShowPrompt = false;
        _hasInteracted = true;
    }

    public void SpawnLoot()
    {
        int randomIndex = Random.Range(0, _componentPrefabs.Length);
        Instantiate(_componentPrefabs[randomIndex], _lootSpawnPos.position, Quaternion.identity);
        _animator.enabled = false;
    }
}
