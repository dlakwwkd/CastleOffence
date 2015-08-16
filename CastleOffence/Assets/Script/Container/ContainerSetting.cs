using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ItemInfo
{
    public GameObject   item;
    public GameObject   icon;
    public int          amount;
    public float        xSize;
    public float        ySize;
}

public class ContainerSetting : MonoBehaviour
{
    public GameObject       itemType = null;
    public List<ItemInfo>   itemList = new List<ItemInfo>();

    GameObject  _grid   = null;
    UIWidget    _widget = null;
    bool        _isOn   = false;

    void Start()
    {
        _widget = GetComponent<UIWidget>();
        _widget.alpha = 0.0f;

        _grid = transform.FindChild("Scroll View").FindChild("Grid").gameObject;
        SettingItems();
    }

    public void OnOff()
    {
        if(_isOn)
        {
            _widget.alpha = 0.0f;
            _isOn = false;
        }
        else
        {
            _widget.alpha = 1.0f;
            _isOn = true;
        }
    }

    void SettingItems()
    {
        for(int i = 0; i < itemList.Count; ++i)
        {
            var itemInfo = itemList[i];

            var newItem = Instantiate(itemType) as GameObject;
            newItem.transform.SetParent(_grid.transform);
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;
            newItem.transform.localScale = Vector3.one;
            newItem.name = itemType.name + "_" + i;

            var dragDrop = newItem.GetComponent<DragDropItem>();
            dragDrop.prefab = itemInfo.item;
            dragDrop.amount = itemInfo.amount;
            dragDrop.xSize = itemInfo.xSize;
            dragDrop.ySize = itemInfo.ySize;

            var icon = Instantiate(itemInfo.icon) as GameObject;
            icon.transform.SetParent(newItem.transform);
            icon.transform.localPosition = Vector3.zero;
            icon.transform.localRotation = Quaternion.identity;
            icon.transform.localScale = Vector3.one;
        }
    }
}
