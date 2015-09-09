using UnityEngine;
using System.Collections;

public class UnitItemInfo : MonoBehaviour
{
    public GameObject prefab = null;
    public GameObject coolTimeBox = null;

    ObjectStatus    _status     = null;
    float           _coolTime   = 0.0f;
    bool            _isOn       = true;

    void Start()
    {
        _status = prefab.GetComponent<ObjectStatus>();
        _coolTime = _status.createTime;

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
        unit.transform.position = new Vector3(0.0f, 5.0f);
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

        while (_coolTime > 0.01f)
        {
            _coolTime -= 0.01f;
            texture.fillAmount = _coolTime / _status.createTime;
            yield return new WaitForSeconds(0.01f);
        }
        _coolTime = _status.createTime;
        _isOn = true;
        Destroy(box);
        yield return null;
    }
}
