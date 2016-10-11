using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }



    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
        //Application.LoadLevel("Game");
    }

    public void PopGameScene()
    {
        SceneManager.LoadScene("Main");
        //Application.LoadLevel("Main");
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
