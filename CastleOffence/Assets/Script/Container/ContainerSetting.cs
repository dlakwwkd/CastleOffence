using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

[System.Serializable]
public struct ItemInfo
{
    public GameObject   item;
    public GameObject   icon;
    public int          amount;
}

public class ContainerSetting : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // inspector field
    [FormerlySerializedAs("itemType")]
    public GameObject ItemType = null;

    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        widget = GetComponent<UIWidget>();
        widget.alpha = 0.0f;

        grid = transform.FindChild("Scroll View").FindChild("Grid").gameObject;
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void OnOff()
    {
        if (isOn)
        {
            widget.alpha = 0.0f;
            isOn = false;
        }
        else
        {
            widget.alpha = 1.0f;
            isOn = true;
        }
    }

    public void SettingBrriers(List<ItemInfo> itemList)
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            var itemInfo = itemList[i];

            for (int xSize = 1; xSize <= 3; ++xSize)
            {
                for (int ySize = 1; ySize <= 3; ++ySize)
                {
                    if (xSize > 1 && ySize > 1)
                        break;

                    var newItem = Instantiate(ItemType) as GameObject;
                    newItem.transform.SetParent(grid.transform);
                    newItem.name = ItemType.name + "_" + itemInfo.item.name + "_" + xSize + ySize;
                    TransformInit(newItem.transform);

                    var dragDrop = newItem.GetComponent<DragDropItem>();
                    dragDrop.Prefab = itemInfo.item;
                    dragDrop.Amount = itemInfo.amount;
                    dragDrop.XSize = xSize;
                    dragDrop.YSize = ySize;

                    var icon = new GameObject();
                    icon.transform.SetParent(newItem.transform);
                    icon.name = itemInfo.item.name;
                    icon.layer = gameObject.layer;
                    TransformInit(icon.transform);

                    var item = ObjectManager.instance.Assign(itemInfo.item.name);
                    {
                        var texture = icon.AddComponent<UITexture>();
                        texture.mainTexture = item.GetComponent<MeshRenderer>().material.mainTexture;
                        texture.uvRect = new Rect(Vector2.zero, new Vector2(xSize, ySize));

                        var w = 80;
                        var h = 80;
                        var aspect = (float)xSize / ySize;
                        if (aspect > 1)
                            h = (int)(h / aspect);
                        else
                            w = (int)(w * aspect);

                        texture.SetDimensions(w, h);
                    }
                    ObjectManager.instance.Free(item);
                }
            }
        }
    }

    public void SettingTowers(List<ItemInfo> itemList)
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            var itemInfo = itemList[i];

            var newItem = Instantiate(ItemType) as GameObject;
            newItem.transform.SetParent(grid.transform);
            newItem.name = ItemType.name + "_" + i;
            TransformInit(newItem.transform);

            var dragDrop = newItem.GetComponent<DragDropItem>();
            dragDrop.Prefab = itemInfo.item;
            dragDrop.Amount = itemInfo.amount;
            dragDrop.XSize = 1.0f;
            dragDrop.YSize = 1.0f;

            var icon = Instantiate(itemInfo.icon) as GameObject;
            icon.transform.SetParent(newItem.transform);
            TransformInit(icon.transform);
        }
    }

    //-----------------------------------------------------------------------------------
    // private functions
    void TransformInit(Transform t)
    {
        t.transform.localPosition = Vector3.zero;
        t.transform.localRotation = Quaternion.identity;
        t.transform.localScale = Vector3.one;
    }
    
    //-----------------------------------------------------------------------------------
    // private field
    GameObject  grid    = null;
    UIWidget    widget  = null;
    bool        isOn    = false;
}
