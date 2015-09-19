using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static GameManager _instance = null;
    public static GameManager instance { get { return _instance; } }

    CameraMove _cameraInfo  = null;
    GameObject _player      = null;
    GameObject _enemy       = null;

    public GameObject       player { get { return _player; } }
    public GameObject       enemy { get { return _enemy; } }

    public GameObject       castle          = null;
    public Vector2          playerCastlePos = Vector2.zero;
    public Vector2          enemyCastlePos  = Vector2.zero;

    public List<GameObject> playerObjList   = new List<GameObject>();
    public List<GameObject> enemyObjList    = new List<GameObject>();

    public List<ItemInfo>   barrierList     = new List<ItemInfo>();
    public List<ItemInfo>   towerList       = new List<ItemInfo>();
    public List<UnitInfo>   unitList        = new List<UnitInfo>();

    public List<GameObject> enemyUnitList   = new List<GameObject>();


    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    void Start()
    {
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

        _enemy.AddComponent<EnemyAI>();
        _enemy.GetComponent<EnemyAI>().unitList = enemyUnitList;

        playerCastlePos = new Vector2(_cameraInfo.leftSide + 5.0f, 2.5f);
        enemyCastlePos = new Vector2(_cameraInfo.rightSide - 5.0f, 2.5f);

        var castleA = Instantiate(castle) as GameObject;
        castleA.transform.SetParent(_player.transform);
        castleA.transform.localPosition = playerCastlePos;
        castleA.name = "PlayerCastle";
        castleA.GetComponent<ObjectStatus>().owner = PlayerType.PLAYER;

        var castleB = Instantiate(castle) as GameObject;
        castleB.transform.SetParent(_enemy.transform);
        castleB.transform.localPosition = enemyCastlePos;
        castleB.name = "EnemyCastle";
        castleB.GetComponent<ObjectStatus>().owner = PlayerType.ENEMY;

        playerObjList.Add(castleA);
        enemyObjList.Add(castleB);

        var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
        var barrier = uiRoot.transform.FindChild("BarrierButton").FindChild("Container");
        var tower = uiRoot.transform.FindChild("TowerButton").FindChild("Container");
        var unit = uiRoot.transform.FindChild("UnitContainer");
        barrier.GetComponent<ContainerSetting>().SettingBrriers(barrierList);
        tower.GetComponent<ContainerSetting>().SettingTowers(towerList);
        unit.GetComponent<UnitSetting>().SettingUnits(unitList);
    }
    public void StartGame()
    {
        
    }
}
