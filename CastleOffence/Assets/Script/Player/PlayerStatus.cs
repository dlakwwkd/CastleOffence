﻿using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    public enum PlayerType
    {
        NONE,
        PLAYER,
        ENEMY,
    }

    public PlayerType   type = PlayerType.NONE;

    UILabel _coinLabel          = null;
    UILabel _incomeUpLabel      = null;
    UILabel _speedUpLabel       = null;
    int     _coin               = 0;
    int     _income             = 10;
    int     _incomeUp           = 10;
    float   _incomeRate         = 3.0f;

    int     _incomeAmountUpCost = 100;
    int     _incomeSpeedUpCost  = 100;


    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Start()
    {
        if (type == PlayerType.PLAYER)
        {
            var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
            _coinLabel = uiRoot.transform.FindChild("OptionPanel").FindChild("PlayerStatusBar").FindChild("CoinAmount").GetComponent<UILabel>();
            _coinLabel.text = _coin.ToString();

            var button1 = uiRoot.transform.FindChild("IncomUpButton");
            _incomeUpLabel = button1.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>();
            _incomeUpLabel.text = _incomeAmountUpCost.ToString();

            var button2 = uiRoot.transform.FindChild("SpeedUpButton");
            _speedUpLabel = button2.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>();
            _speedUpLabel.text = _incomeSpeedUpCost.ToString();

            UIEventListener.Get(button1.gameObject).onClick += IncomeUp;
            UIEventListener.Get(button2.gameObject).onClick += SpeedUp;
        }
        StartCoroutine("Incom");
    }


    void IncomeUp(GameObject sender)
    {
        if(Purchase(_incomeAmountUpCost))
        {
            _income += _incomeUp;
            _incomeUp += 10;
            _incomeAmountUpCost = _income * 10;
            _incomeUpLabel.text = _incomeAmountUpCost.ToString();
        }
    }
    void SpeedUp(GameObject sender)
    {
        if(Purchase(_incomeSpeedUpCost))
        {
            _incomeRate -= 0.2f;
            _incomeSpeedUpCost += (int)(_incomeSpeedUpCost * 0.5f);
            _speedUpLabel.text = _incomeSpeedUpCost.ToString();
        }
    }


    public void Init(int money)
    {
        _coin = money;
    }
    public bool Purchase(int cost)
    {
        if(cost <= _coin)
        {
            _coin -= cost;

            if (type == PlayerType.PLAYER)
                _coinLabel.text = _coin.ToString();
            return true;
        }
        return false;
    }
    public void Reward(int money)
    {
        _coin += money;

        if(type == PlayerType.PLAYER)
            _coinLabel.text = _coin.ToString();
    }


    IEnumerator Incom()
    {
        while(true)
        {
            yield return new WaitForSeconds(_incomeRate);
            Reward(_income);
        }
    }
}
