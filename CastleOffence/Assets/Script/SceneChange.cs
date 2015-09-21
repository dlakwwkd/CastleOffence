using UnityEngine;

public class SceneChange : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


    public void LoadGameScene()
    {
        Application.LoadLevel("Game");
    }
    public void PopGameScene()
    {
        Application.LoadLevel("Main");
    }
    public void CloseApp()
    {
        Application.Quit();
    }
}
