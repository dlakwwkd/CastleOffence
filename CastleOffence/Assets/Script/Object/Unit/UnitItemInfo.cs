using UnityEngine;
using System.Collections;

public class UnitItemInfo : MonoBehaviour
{
    public GameObject prefab = null;
    public GameObject coolTimeBox = null;

    ObjectStatus    _unitInfo     = null;
    Vector2         _createPos  = Vector2.zero;
    float           _coolTime   = 0.0f;
    bool            _isOn       = true;

    void Start()
    {
        _unitInfo = prefab.GetComponent<ObjectStatus>();
        _createPos = GameManager.instance.playerCastlePos;
        _coolTime = _unitInfo.createTime;

        UIEventListener.Get(gameObject).onClick += onClick;
        UIEventListener.Get(gameObject).onPress += onPress;
    }

    void onClick(GameObject sender)
    {
        if (_isOn)
        {
            _isOn = false;
            ProduceUnit();
            StartCoroutine("CoolTimeProcess");
        }
    }
    void onPress(GameObject sender, bool isDown)
    {
        if (isDown)
            Camera.main.GetComponent<CameraMove>().Lock();
        else
            Camera.main.GetComponent<CameraMove>().UnLock();
    }


    void ProduceUnit()
    {
        var unit = ObjectManager.instance.Assign(prefab.name);
        unit.transform.position = _createPos;

        var status = unit.GetComponent<ObjectStatus>();
        status.owner = PlayerType.PLAYER;
    }

    IEnumerator CoolTimeProcess()
    {
        var box = Instantiate(coolTimeBox) as GameObject;
        box.transform.SetParent(transform);
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = Quaternion.identity;
        box.transform.localScale = Vector3.one;

        var texture = box.GetComponent<UITexture>();
        texture.alpha = 0.8f;
        texture.depth = 10;

        while (_coolTime > 0.1f)
        {
            _coolTime -= Time.deltaTime;
            texture.fillAmount = _coolTime / _unitInfo.createTime;
            yield return new WaitForEndOfFrame();
        }
        _coolTime = _unitInfo.createTime;
        _isOn = true;
        Destroy(box);
        yield return null;
    }
}
