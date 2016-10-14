using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void PopGameScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
