using UnityEngine;
using System.Collections;
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

public class PlayerStatus : MonoBehaviour
{
    public int gold = 0;
    public int wood = 0;

    public List<ItemInfo> barrierList   = new List<ItemInfo>();
    public List<ItemInfo> towerList     = new List<ItemInfo>();

    void        Start()
    {
        StartCoroutine(EnterGame());
    }

    IEnumerator EnterGame()
    {
        yield return new WaitForEndOfFrame();

        var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
        var barrier = uiRoot.transform.FindChild("BarrierButton").FindChild("BarrierContainer");
        var Tower = uiRoot.transform.FindChild("TowerButton").FindChild("TowerContainer");
        barrier.GetComponent<ContainerSetting>().SettingItems(barrierList);
        Tower.GetComponent<ContainerSetting>().SettingItems(towerList);
    }
}
