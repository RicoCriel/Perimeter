using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager Instance;
    [SerializeField]
    private PostProcessProfile _postProcessProfile;
    private Vignette _vignette;
    private float _originalIntensity;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _vignette = _postProcessProfile.GetSetting<Vignette>();
        _originalIntensity = _vignette.intensity.value;
    }

    public void PlayVignette()
    {
        if (_vignette == null) return;

        _vignette.active = true;
        _vignette.intensity.overrideState = true;

        DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, 0.4f, 0.3f)
            .OnComplete(() =>
            {
                DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, _originalIntensity, 0.3f);
            });
    }

    private void DisableVignette()
    {
        if (_vignette == null) return;
        _vignette.active = false;
    }
}
