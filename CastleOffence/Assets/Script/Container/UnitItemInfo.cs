using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

public class UnitItemInfo : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // inspector field
    [FormerlySerializedAs("prefab")]
    public GameObject Prefab        = null;
    [FormerlySerializedAs("coolTimeBox")]
    public GameObject CoolTimeBox   = null;

    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        unitInfo = Prefab.GetComponent<ObjectStatus>();
        player = GameManager.instance.player;
        createPos = GameManager.instance.playerCastlePos;
        coolTime = unitInfo.CreateTime;
        cost = unitInfo.Cost;

        var label = transform.FindChild("Cost").GetComponent<UILabel>();
        label.text = cost.ToString();
        label.depth = 11;

        UIEventListener.Get(gameObject).onClick += onClick;
        UIEventListener.Get(gameObject).onPress += onPress;
    }

    void onClick(GameObject sender)
    {
        if (isOn)
        {
            if (player.Purchase(cost))
            {
                isOn = false;
                ProduceUnit();
                StartCoroutine("CoolTimeProcess");
                AudioManager.instance.PlayPurchaseUnit();
            }
            else
                AudioManager.instance.PlayPurchaseFail();
        }
        else
            AudioManager.instance.PlayPurchaseFail();
    }

    void onPress(GameObject sender, bool isDown)
    {
        if (isDown)
            Camera.main.GetComponent<CameraMove>().Lock();
        else
            Camera.main.GetComponent<CameraMove>().UnLock();
    }

    //-----------------------------------------------------------------------------------
    // private functions
    void ProduceUnit()
    {
        var unit = ObjectManager.instance.Assign(Prefab.name);
        unit.transform.position = createPos;
        unit.transform.localRotation = Quaternion.identity;

        var status = unit.GetComponent<ObjectStatus>();
        status.Owner = PlayerStatus.PlayerType.PLAYER;
        status.ChangeDir(ObjectStatus.Direction.RIGHT);

        GameManager.instance.playerObjList.Add(unit);
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator CoolTimeProcess()
    {
        var box = Instantiate(CoolTimeBox) as GameObject;
        box.transform.SetParent(transform);
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = Quaternion.identity;
        box.transform.localScale = Vector3.one;

        var texture = box.GetComponent<UITexture>();
        texture.alpha = 0.8f;
        texture.depth = 10;

        while (coolTime > 0.1f)
        {
            coolTime -= Time.deltaTime;
            texture.fillAmount = coolTime / unitInfo.CreateTime;
            yield return new WaitForEndOfFrame();
        }
        coolTime = unitInfo.CreateTime;
        isOn = true;
        Destroy(box);
    }
    
    //-----------------------------------------------------------------------------------
    // private field
    ObjectStatus    unitInfo    = null;
    PlayerStatus    player      = null;
    Vector2         createPos   = Vector2.zero;
    float           coolTime    = 0.0f;
    int             cost        = 0;
    bool            isOn        = true;
}
