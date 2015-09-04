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


}
