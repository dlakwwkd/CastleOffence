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
    public List<ItemInfo> itemList = new List<ItemInfo>();

    UIWidget    _widget = null;
    bool        _isOn   = false;

    void Start()
    {
        _widget = GetComponent<UIWidget>();
        _widget.alpha = 0.0f;
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
}
