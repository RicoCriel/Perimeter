using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CrossFade : SceneTransition
{
    [SerializeField] private CanvasGroup _crossFade;

    public override IEnumerator AnimateTransitionIn()
    {
        var tweener = _crossFade.DOFade(1f, 1f);
        yield return tweener.WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = _crossFade.DOFade(0f, 1f);
        yield return tweener.WaitForCompletion();
    }
}
