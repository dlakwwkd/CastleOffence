using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    // inspector
    public GameObject       LabelPrefab     = null;
    public GameObject       CastlePrefab    = null;
    public List<ItemInfo>   BarrierList     = new List<ItemInfo>();
    public List<ItemInfo>   TowerList       = new List<ItemInfo>();
    public List<UnitInfo>   UnitList        = new List<UnitInfo>();
    public List<GameObject> EnemyUnitList   = new List<GameObject>();
    public List<GameObject> EnemyTowerList  = new List<GameObject>();

    // property
    public PlayerStatus     mPlayer { get; private set; }
    public PlayerStatus     mEnemy { get; private set; }
    public Vector2          mPlayerCastlePos { get; private set; }
    public Vector2          mEnemyCastlePos { get; private set; }
    public List<GameObject> mPlayerObjList { get; private set; }
    public List<GameObject> mEnemyObjList { get; private set; }

    // private
    CameraMove  _cameraInfo = null;
    Camera      _nguiCam    = null;
    UIRoot      _uiRoot     = null;


    void Awake()
    {
        if (instance == null)
            instance = this;

        mPlayerCastlePos = Vector2.zero;
        mEnemyCastlePos = Vector2.zero;
        mPlayerObjList = new List<GameObject>();
        mEnemyObjList = new List<GameObject>();
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
        StartCoroutine(UpScrollingLabelUI(uiPos, 50, text, Color.yellow, 3, 30.0f));
    }



    void PlayerSetting()
    {
        var player = new GameObject();
        var enemy = new GameObject();

        player.name = "Player";
        enemy.name = "Enemy";

        mPlayer = player.AddComponent<PlayerStatus>();
        mEnemy = enemy.AddComponent<PlayerStatus>();

        mPlayer.type = PlayerStatus.PlayerType.PLAYER;
        mEnemy.type = PlayerStatus.PlayerType.ENEMY;

        mPlayer.Init(3000);
        mEnemy.Init(3000);

        var ai = enemy.AddComponent<EnemyAI>();
        ai.UnitList = EnemyUnitList;
        ai.TowerList = EnemyTowerList;
    }

    void CastleSetting()
    {
        mPlayerCastlePos = new Vector2(_cameraInfo.leftSide + 5.0f, 2.0f);
        mEnemyCastlePos = new Vector2(_cameraInfo.rightSide - 5.0f, 2.0f);

        var castleA = Instantiate(CastlePrefab) as GameObject;
        castleA.transform.SetParent(mPlayer.transform);
        castleA.transform.localPosition = new Vector3(mPlayerCastlePos.x, mPlayerCastlePos.y - 1.5f, 2.0f);
        castleA.name = "PlayerCastle";
        castleA.GetComponent<ObjectStatus>().owner = PlayerStatus.PlayerType.PLAYER;

        var castleB = Instantiate(CastlePrefab) as GameObject;
        castleB.transform.SetParent(mEnemy.transform);
        castleB.transform.localPosition = new Vector3(mEnemyCastlePos.x, mEnemyCastlePos.y - 1.5f, 2.0f);
        castleB.name = "EnemyCastle";
        castleB.GetComponent<ObjectStatus>().owner = PlayerStatus.PlayerType.ENEMY;

        mPlayerObjList.Add(castleA);
        mEnemyObjList.Add(castleB);
    }

    void ItemListSetting()
    {
        var barrier = _uiRoot.transform.FindChild("BarrierButton").FindChild("Container");
        var tower = _uiRoot.transform.FindChild("TowerButton").FindChild("Container");
        var unit = _uiRoot.transform.FindChild("UnitContainer");
        barrier.GetComponent<ContainerSetting>().SettingBrriers(BarrierList);
        tower.GetComponent<ContainerSetting>().SettingTowers(TowerList);
        unit.GetComponent<UnitSetting>().SettingUnits(UnitList);
    }



    IEnumerator UpScrollingLabel(Vector3 worldPos, int fontSize, string text, Color color, int depth, float speed)
    {
        var obj = ObjectManager.instance.Assign(LabelPrefab.name);
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
            pos -= new Vector3(Screen.width, Screen.height) * 0.5f;
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
        var obj = ObjectManager.instance.Assign(LabelPrefab.name);
        obj.transform.SetParent(_uiRoot.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        var label = obj.GetComponent<UILabel>();
        label.fontSize = fontSize;
        label.text = text;
        label.color = color;
        label.depth = depth;

        speed *=  (float)Screen.height / _uiRoot.manualHeight;
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
