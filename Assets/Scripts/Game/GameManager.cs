using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //refactor entire class to be responsible for game states only
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}
