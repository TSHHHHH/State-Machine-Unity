using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocater.RegisterService<GameMaster>(this);
    }

    // Update is called once per frame
    void Update()
    {
        // if player presses "P" key, restart the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            ReloadScene();
        }

        // if player presses "escape" key, quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnDisable()
    {
        ServiceLocater.UnregisterService<GameMaster>();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
