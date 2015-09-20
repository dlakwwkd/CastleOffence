using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerType
{
    NONE,
    PLAYER,
    ENEMY,
}

public class PlayerStatus : MonoBehaviour
{
    public PlayerType   type = PlayerType.NONE;

    UILabel _coinLabel  = null;
    int     _coin       = 1000;

    void Start()
    {
        var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
        _coinLabel = uiRoot.transform.FindChild("OptionPanel").FindChild("PlayerStatusBar").FindChild("CoinAmount").GetComponent<UILabel>();
        _coinLabel.text = _coin.ToString();
    }
    
    public bool Purchase(int cost)
    {
        if(cost < _coin)
        {
            _coin -= cost;
            _coinLabel.text = _coin.ToString();
            return true;
        }
        return false;
    }
}
