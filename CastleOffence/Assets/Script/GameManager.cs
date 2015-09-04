using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static GameManager _instance = null;
    public static GameManager instance { get { return _instance; } }

    GameObject _player = null;
    GameObject _enemy = null;

    public GameObject       castle      = null;
    public List<ItemInfo>   barrierList = new List<ItemInfo>();

    void Start()
    {
        if (_instance == null)
            _instance = this;

        InitGame();
        StartGame();
    }

    public void InitGame()
    {
        _player = new GameObject();
        _enemy = new GameObject();

        _player.name = "Player";
        _enemy.name = "Enemy";

        _player.AddComponent<PlayerStatus>();
        _enemy.AddComponent<PlayerStatus>();



        var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
        var barrier = uiRoot.transform.FindChild("BarrierButton").FindChild("BarrierContainer");
        barrier.GetComponent<ContainerSetting>().SettingItems(barrierList);
    }
    public void StartGame()
    {

    }
}
