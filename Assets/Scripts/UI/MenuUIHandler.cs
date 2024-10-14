using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _creditsButton;

    private void Start()
    {
        _startButton.onClick.AddListener(LoadGame);
        _quitButton.onClick.AddListener(ExitGame);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }


}
