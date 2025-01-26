using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _creditsButton;

    private void Start()
    {
        _startButton.onClick.AddListener(Play);
        _quitButton.onClick.AddListener(Quit);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(Play);
        _quitButton.onClick.RemoveListener(Quit);
    }

    private void Play()
    {
        LevelManager.Instance.LoadLevel("NavigationMenu", "CrossFade");
    }

    private void Quit()
    {
        LevelManager.Instance.ExitGame();
    }
}
