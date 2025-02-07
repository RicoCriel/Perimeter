using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InteractView : MonoBehaviour
{
    public TMP_Text InteractionText;
    public RectTransform InteractionPanel;
    public bool ShouldShowPrompt;
    [SerializeField] private Vector3 _worldOffset;
    [SerializeField] private Transform _lookAtTarget;
    private Camera _mainCamera;
    private Outline _outline;

    public UnityEvent OnShowInteraction;
    public UnityEvent OnHideInteraction;

    private void Start()
    {
        _mainCamera = Camera.main;
        InteractionPanel.gameObject.SetActive(false);
        _outline = GetComponent<Outline>();
        _outline.OutlineWidth = 0f;
    }

    private void Update()
    {
        UpdateUIPosition();
    }

    public void ShowInteractionUI(string text)
    {
        InteractionText.text = text;
        InteractionPanel.gameObject.SetActive(true);
        _outline.OutlineWidth = 3f;
    }

    public void HideInteractionUI()
    {
        InteractionPanel.gameObject.SetActive(false);
        _outline.OutlineWidth = 0f;
    }

    private void UpdateUIPosition()
    {
        Vector3 pos = _mainCamera.WorldToScreenPoint(_lookAtTarget.position + _worldOffset);
        if(ShouldShowPrompt)
        {
            if (InteractionPanel.transform.position != pos)
            {
                InteractionPanel.transform.position = pos;
            }
        }
        else
        {
            DisableInteractionUI();
            return;
        }
    }

    private void DisableInteractionUI()
    {
        InteractionPanel.gameObject.SetActive(false);
        _outline.enabled = false;
    }
}

