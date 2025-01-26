using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Slider _loadingBar;
    [SerializeField] private GameObject _levelTransitionsContainer;
    private SceneTransition[] _transitions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        _transitions = _levelTransitionsContainer.GetComponentsInChildren<SceneTransition>();
    }

    public void LoadLevel(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        //go through the transitionarray until we find the first transition that has the transistion name we are looking for
        SceneTransition transition = _transitions.First(t => t.name == transitionName);
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        //Do not allow the scene to be activated
        scene.allowSceneActivation = false;
        yield return transition.AnimateTransitionIn();

        _loadingBar.gameObject.SetActive(true);

        do
        {
            //fill the progress bar until the scene is fully loaded
            _loadingBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
        _loadingBar.gameObject.SetActive(false);

        yield return transition.AnimateTransitionOut();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
