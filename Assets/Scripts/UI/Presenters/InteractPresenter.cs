using UnityEngine;
using UnityEngine.Events;

public class InteractPresenter 
{
    //Connects Model and View
    private InteractModel _model;
    private InteractView _view;

    public UnityEvent OnInteractionSet;
    public UnityEvent OnInteractionCleared;

    public InteractPresenter(InteractModel model, InteractView view)
    {
        this._model = model;
        this._view = view;
    }

    public void SetInteraction(string text)
    {
        _model.SetInteractionText(text);
        _view.ShowInteraction(text);
    }

    public void ClearInteraction()
    {
        _view.HideInteraction();
    }
}
