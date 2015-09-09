﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static GameManager _instance = null;
    public static GameManager instance { get { return _instance; } }

    CameraMove _cameraInfo  = null;
    GameObject _player      = null;
    GameObject _enemy       = null;

    public GameObject       castle      = null;
    public List<ItemInfo>   barrierList = new List<ItemInfo>();
    public List<ItemInfo>   towerList = new List<ItemInfo>();
    public List<UnitInfo>   unitList = new List<UnitInfo>();

    void Start()
    {
        if (_instance == null)
            _instance = this;

        InitGame();
        StartGame();
    }

    public void InitGame()
    {
        _cameraInfo = Camera.main.GetComponent<CameraMove>();
        _player = new GameObject();
        _enemy = new GameObject();

        _player.name = "Player";
        _enemy.name = "Enemy";

        _player.AddComponent<PlayerStatus>();
        _enemy.AddComponent<PlayerStatus>();

        _player.GetComponent<PlayerStatus>().type = PlayerType.PLAYER;
        _enemy.GetComponent<PlayerStatus>().type = PlayerType.ENEMY;

        var castleA = Instantiate(castle) as GameObject;
        castleA.transform.SetParent(_player.transform);
        castleA.transform.localPosition = new Vector3(_cameraInfo.leftSide + 5.0f, 2.5f);
        castleA.name = "PlayerCastle";
        castleA.GetComponent<ObjectStatus>().owner = PlayerType.PLAYER;

        var castleB = Instantiate(castle) as GameObject;
        castleB.transform.SetParent(_enemy.transform);
        castleB.transform.localPosition = new Vector3(_cameraInfo.rightSide - 5.0f, 2.5f);
        castleB.name = "EnemyCastle";
        castleB.GetComponent<ObjectStatus>().owner = PlayerType.ENEMY;

        var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
        var barrier = uiRoot.transform.FindChild("BarrierButton").FindChild("Container");
        var tower = uiRoot.transform.FindChild("TowerButton").FindChild("Container");
        var unit = uiRoot.transform.FindChild("UnitContainer");
        barrier.GetComponent<ContainerSetting>().SettingItems(barrierList);
        tower.GetComponent<ContainerSetting>().SettingItems(towerList);
        unit.GetComponent<UnitSetting>().SettingItems(unitList);
    }
    public void StartGame()
    {
        
    }
}