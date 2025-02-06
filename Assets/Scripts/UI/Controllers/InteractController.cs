using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    //Handles player interactions
    private InteractPresenter _presenter;
    [SerializeField] private string _interactionPrompt;
    private InteractView _view;

    private void Start()
    {
        InteractModel model = new InteractModel();
        _view = GetComponent<InteractView>();
        _presenter = new InteractPresenter(model, _view);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _presenter.SetInteraction(_interactionPrompt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _presenter.ClearInteraction();
        }
    }
}
