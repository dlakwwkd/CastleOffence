using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static GameManager          _instance = null;
    public static GameManager   instance { get { return _instance; } }

    public UIRoot               uiRoot { get { return _uiRoot; } }
    public PlayerStatus         player { get { return _player; } }
    public PlayerStatus         enemy { get { return _enemy; } }
    public List<GameObject>     playerObjList { get { return _playerObjList; } }
    public List<GameObject>     enemyObjList { get { return _enemyObjList; } }

    public GameObject           labelPrefab     = null;
    public GameObject           castle          = null;

    public Vector2              playerCastlePos = Vector2.zero;
    public Vector2              enemyCastlePos  = Vector2.zero;

    public List<ItemInfo>       barrierList     = new List<ItemInfo>();
    public List<ItemInfo>       towerList       = new List<ItemInfo>();
    public List<UnitInfo>       unitList        = new List<UnitInfo>();

    public List<GameObject>     enemyUnitList   = new List<GameObject>();
    public List<GameObject>     enemyTowerList  = new List<GameObject>();

    CameraMove          _cameraInfo     = null;
    Camera              _nguiCam        = null;
    UIRoot              _uiRoot         = null;
    PlayerStatus        _player         = null;
    PlayerStatus        _enemy          = null;
    List<GameObject>    _playerObjList  = new List<GameObject>();
    List<GameObject>    _enemyObjList   = new List<GameObject>();


    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Start()
    {
        InitGame();
        StartGame();
    }


    public void InitGame()
    {
        _cameraInfo = Camera.main.GetComponent<CameraMove>();
        _nguiCam = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("NGUI"));
        _uiRoot = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<UIRoot>();

        ItemListSetting();
        PlayerSetting();
        CastleSetting();
    }
    public void StartGame()
    {
    }
    public void RewardLabelShow(Vector3 worldPos, int reward)
    {
        worldPos += Vector3.right * 0.5f;
        var text = "+" + reward.ToString();
        StartCoroutine(UpScrollingLabel(worldPos, 35, text, Color.yellow, 1, 1.0f));
    }
    public void DamageLabelShow(Vector3 worldPos, int damage)
    {
        var text = "-" + damage.ToString();
        StartCoroutine(UpScrollingLabel(worldPos, 25, text, Color.red, 0, 1.5f));
    }
    public void IncomeLabelShow(Vector3 uiPos, int income)
    {
        var text = "+" + income.ToString();
        StartCoroutine(UpScrollingLabelUI(uiPos, 50, text, Color.yellow, 3, 5.0f));
    }



    void PlayerSetting()
    {
        var player = new GameObject();
        var enemy = new GameObject();

        player.name = "Player";
        enemy.name = "Enemy";

        _player = player.AddComponent<PlayerStatus>();
        _enemy = enemy.AddComponent<PlayerStatus>();

        _player.type = PlayerStatus.PlayerType.PLAYER;
        _enemy.type = PlayerStatus.PlayerType.ENEMY;

        _player.Init(300);
        _enemy.Init(300);

        var ai = enemy.AddComponent<EnemyAI>();
        ai.unitList = enemyUnitList;
        ai.towerList = enemyTowerList;
    }
    void CastleSetting()
    {
        playerCastlePos = new Vector2(_cameraInfo.leftSide + 5.0f, 2.0f);
        enemyCastlePos = new Vector2(_cameraInfo.rightSide - 5.0f, 2.0f);

        var castleA = Instantiate(castle) as GameObject;
        castleA.transform.SetParent(_player.transform);
        castleA.transform.localPosition = new Vector3(playerCastlePos.x, playerCastlePos.y - 1.5f, 2.0f);
        castleA.name = "PlayerCastle";
        castleA.GetComponent<ObjectStatus>().owner = PlayerStatus.PlayerType.PLAYER;

        var castleB = Instantiate(castle) as GameObject;
        castleB.transform.SetParent(_enemy.transform);
        castleB.transform.localPosition = new Vector3(enemyCastlePos.x, enemyCastlePos.y - 1.5f, 2.0f);
        castleB.name = "EnemyCastle";
        castleB.GetComponent<ObjectStatus>().owner = PlayerStatus.PlayerType.ENEMY;

        _playerObjList.Add(castleA);
        _enemyObjList.Add(castleB);
    }
    void ItemListSetting()
    {
        var barrier = _uiRoot.transform.FindChild("BarrierButton").FindChild("Container");
        var tower = _uiRoot.transform.FindChild("TowerButton").FindChild("Container");
        var unit = _uiRoot.transform.FindChild("UnitContainer");
        barrier.GetComponent<ContainerSetting>().SettingBrriers(barrierList);
        tower.GetComponent<ContainerSetting>().SettingTowers(towerList);
        unit.GetComponent<UnitSetting>().SettingUnits(unitList);
    }


    IEnumerator UpScrollingLabel(Vector3 worldPos, int fontSize, string text, Color color, int depth, float speed)
    {
        var obj = ObjectManager.instance.Assign(labelPrefab.name);
        obj.transform.SetParent(_uiRoot.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        var label = obj.GetComponent<UILabel>();
        label.fontSize = fontSize;
        label.text = text;
        label.color = color;
        label.depth = depth;

        float time = 2.0f / speed;
        while(time > 0.0f)
        {
            time -= Time.deltaTime;
            worldPos += Vector3.up * speed * Time.deltaTime;

            var pos = _nguiCam.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(worldPos));
            pos -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            pos *= _uiRoot.manualHeight / Screen.height;
            pos.z = 0.0f;

            label.fontSize = (int)(fontSize * (8.0f / Camera.main.orthographicSize));

            obj.transform.localPosition = pos;
            if(time < 1.0f)
                label.alpha = time;

            yield return new WaitForEndOfFrame();
        }
        ObjectManager.instance.Free(obj);
    }
    IEnumerator UpScrollingLabelUI(Vector3 uiPos, int fontSize, string text, Color color, int depth, float speed)
    {
        var obj = ObjectManager.instance.Assign(labelPrefab.name);
        obj.transform.SetParent(_uiRoot.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        var label = obj.GetComponent<UILabel>();
        label.fontSize = fontSize;
        label.text = text;
        label.color = color;
        label.depth = depth;

        speed *= _uiRoot.manualHeight / Screen.height;
        float time = 1.0f;
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            uiPos += Vector3.up * speed * Time.deltaTime;

            obj.transform.localPosition = uiPos;
            if (time < 0.5f)
                label.alpha = time;

            yield return new WaitForEndOfFrame();
        }
        ObjectManager.instance.Free(obj);
    }
}
