using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct UnitInfo
{
    public GameObject item;
    public GameObject icon;
}

public class UnitSetting : MonoBehaviour
{
    public GameObject itemType = null;

    GameObject _grid = null;

    void Start()
    {
        _grid = transform.FindChild("Grid").gameObject;
    }

    public void SettingItems(List<UnitInfo> itemList)
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            var info = itemList[i];
            var slot = _grid.transform.FindChild("UnitSlot_" + i.ToString());

            var newItem = Instantiate(itemType) as GameObject;
            newItem.transform.SetParent(slot.transform);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;
            newItem.transform.localScale = Vector3.one;
            newItem.name = itemType.name + "_" + i;
            newItem.GetComponent<UISprite>().depth = 2;

            var itemInfo = newItem.GetComponent<UnitItemInfo>();
            itemInfo.prefab = info.item;

            var icon = Instantiate(info.icon) as GameObject;
            icon.transform.SetParent(newItem.transform);
            icon.transform.localPosition = Vector3.zero;
            icon.transform.localRotation = Quaternion.identity;
            icon.transform.localScale = Vector3.one;
            icon.GetComponent<UITexture>().depth = 3;
        }
    }
}
