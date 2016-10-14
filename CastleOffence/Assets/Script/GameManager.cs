using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // property
    public static GameManager   instance { get; private set; }
    public PlayerStatus         player { get; private set; }
    public PlayerStatus         enemy { get; private set; }
    public Vector2              playerCastlePos { get; private set; }
    public Vector2              enemyCastlePos { get; private set; }
    public List<GameObject>     playerObjList { get; private set; }
    public List<GameObject>     enemyObjList { get; private set; }

    //-----------------------------------------------------------------------------------
    // inspector field
    public GameObject           LabelPrefab     = null;
    public GameObject           CastlePrefab    = null;
    public List<ItemInfo>       BarrierList     = new List<ItemInfo>();
    public List<ItemInfo>       TowerList       = new List<ItemInfo>();
    public List<UnitInfo>       UnitList        = new List<UnitInfo>();
    public List<GameObject>     EnemyUnitList   = new List<GameObject>();
    public List<GameObject>     EnemyTowerList  = new List<GameObject>();

    //-----------------------------------------------------------------------------------
    // handler functions
    void Awake()
    {
        if (instance == null)
            instance = this;

        playerCastlePos = Vector2.zero;
        enemyCastlePos = Vector2.zero;
        playerObjList = new List<GameObject>();
        enemyObjList = new List<GameObject>();
    }

    void Start()
    {
        InitGame();
        StartGame();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void InitGame()
    {
        cameraInfo = Camera.main.GetComponent<CameraMove>();
        nguiCam = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("NGUI"));
        uiRoot = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<UIRoot>();

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

    //-----------------------------------------------------------------------------------
    // private functions
    void PlayerSetting()
    {
        var player = new GameObject();
        var enemy = new GameObject();

        player.name = "Player";
        enemy.name = "Enemy";

        this.player = player.AddComponent<PlayerStatus>();
        this.enemy = enemy.AddComponent<PlayerStatus>();

        this.player.type = PlayerStatus.PlayerType.PLAYER;
        this.enemy.type = PlayerStatus.PlayerType.ENEMY;

        this.player.Init(3000);
        this.enemy.Init(3000);

        var ai = enemy.AddComponent<EnemyAI>();
        ai.UnitList = EnemyUnitList;
        ai.TowerList = EnemyTowerList;
    }

    void CastleSetting()
    {
        playerCastlePos = new Vector2(cameraInfo.LeftSide + 5.0f, 2.0f);
        enemyCastlePos = new Vector2(cameraInfo.RightSide - 5.0f, 2.0f);

        var castleA = Instantiate(CastlePrefab) as GameObject;
        castleA.transform.SetParent(player.transform);
        castleA.transform.localPosition = new Vector3(playerCastlePos.x, playerCastlePos.y - 1.5f, 2.0f);
        castleA.name = "PlayerCastle";
        castleA.GetComponent<ObjectStatus>().Owner = PlayerStatus.PlayerType.PLAYER;

        var castleB = Instantiate(CastlePrefab) as GameObject;
        castleB.transform.SetParent(enemy.transform);
        castleB.transform.localPosition = new Vector3(enemyCastlePos.x, enemyCastlePos.y - 1.5f, 2.0f);
        castleB.name = "EnemyCastle";
        castleB.GetComponent<ObjectStatus>().Owner = PlayerStatus.PlayerType.ENEMY;

        playerObjList.Add(castleA);
        enemyObjList.Add(castleB);
    }

    void ItemListSetting()
    {
        var barrier = uiRoot.transform.FindChild("BarrierButton").FindChild("Container");
        var tower = uiRoot.transform.FindChild("TowerButton").FindChild("Container");
        var unit = uiRoot.transform.FindChild("UnitContainer");
        barrier.GetComponent<ContainerSetting>().SettingBrriers(BarrierList);
        tower.GetComponent<ContainerSetting>().SettingTowers(TowerList);
        unit.GetComponent<UnitSetting>().SettingUnits(UnitList);
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator UpScrollingLabel(Vector3 worldPos, int fontSize, string text, Color color, int depth, float speed)
    {
        var obj = ObjectManager.instance.Assign(LabelPrefab.name);
        obj.transform.SetParent(uiRoot.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        var label = obj.GetComponent<UILabel>();
        label.fontSize = fontSize;
        label.text = text;
        label.color = color;
        label.depth = depth;

        float time = 2.0f / speed;
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            worldPos += Vector3.up * speed * Time.deltaTime;

            var pos = nguiCam.ViewportToScreenPoint(Camera.main.WorldToViewportPoint(worldPos));
            pos -= new Vector3(Screen.width, Screen.height) * 0.5f;
            pos *= uiRoot.manualHeight / Screen.height;
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
        obj.transform.SetParent(uiRoot.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        var label = obj.GetComponent<UILabel>();
        label.fontSize = fontSize;
        label.text = text;
        label.color = color;
        label.depth = depth;

        speed *=  (float)Screen.height / uiRoot.manualHeight;
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
    
    //-----------------------------------------------------------------------------------
    // private field
    CameraMove  cameraInfo  = null;
    Camera      nguiCam     = null;
    UIRoot      uiRoot      = null;
}
