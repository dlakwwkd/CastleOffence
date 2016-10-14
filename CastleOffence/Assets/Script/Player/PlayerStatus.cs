using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    public enum PlayerType
    {
        NONE,
        PLAYER,
        ENEMY,
    }

    //-----------------------------------------------------------------------------------
    // property
    public PlayerType type { get; set; }
    
    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        if (type == PlayerType.PLAYER)
        {
            var uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
            statusBar = uiRoot.transform.FindChild("OptionPanel").FindChild("PlayerStatusBar").GetComponent<UIWidget>();
            coinLabel = statusBar.transform.FindChild("CoinAmount").GetComponent<UILabel>();
            coinLabel.text = coin.ToString();

            var button1 = uiRoot.transform.FindChild("IncomUpButton");
            incomeUpLabel = button1.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>();
            incomeUpLabel.text = incomeAmountUpCost.ToString();

            var button2 = uiRoot.transform.FindChild("SpeedUpButton");
            speedUpLabel = button2.FindChild("Animation").FindChild("Cost").GetComponent<UILabel>();
            speedUpLabel.text = incomeSpeedUpCost.ToString();

            UIEventListener.Get(button1.gameObject).onClick += IncomeUp;
            UIEventListener.Get(button2.gameObject).onClick += SpeedUp;
        }
        StartCoroutine("Incom");
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void IncomeUp(GameObject sender)
    {
        if (Purchase(incomeAmountUpCost))
        {
            income += incomeUp;
            incomeUp += 10;
            incomeAmountUpCost = income * 10;
            if (type == PlayerType.PLAYER)
            {
                AudioManager.instance.PlayIncomeUp();
                incomeUpLabel.text = incomeAmountUpCost.ToString();
            }
        }
    }

    public void SpeedUp(GameObject sender)
    {
        if (Purchase(incomeSpeedUpCost))
        {
            incomeRate -= 0.2f;
            if (incomeRate < 0.25f)
            {
                incomeSpeedUpCost = int.MaxValue;
                if (type == PlayerType.PLAYER)
                    speedUpLabel.text = "Max";
                return;
            }
            incomeSpeedUpCost += (int)(incomeSpeedUpCost * 0.5f);
            if (type == PlayerType.PLAYER)
            {
                AudioManager.instance.PlaySpeedUp();
                speedUpLabel.text = incomeSpeedUpCost.ToString();
            }
        }
    }

    public void Init(int money)
    {
        coin = money;
    }

    public bool Purchase(int cost)
    {
        if (cost <= coin)
        {
            coin -= cost;

            if (type == PlayerType.PLAYER)
                coinLabel.text = coin.ToString();
            return true;
        }
        return false;
    }

    public void Reward(int money)
    {
        coin += money;

        if (type == PlayerType.PLAYER)
            coinLabel.text = coin.ToString();
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator Incom()
    {
        while (true)
        {
            yield return new WaitForSeconds(incomeRate);

            if (type == PlayerType.PLAYER)
            {
                var pos = statusBar.transform.localPosition;
                pos += new Vector3(statusBar.localSize.x * 0.3f, -(statusBar.localSize.y * 0.6f));
                GameManager.instance.IncomeLabelShow(pos, income);
                AudioManager.instance.PlayCoinUp();
            }
            Reward(income);
        }
    }
    
    //-----------------------------------------------------------------------------------
    // private field
    UIWidget    statusBar           = null;
    UILabel     coinLabel           = null;
    UILabel     incomeUpLabel       = null;
    UILabel     speedUpLabel        = null;
    int         coin                = 0;
    int         income              = 10;
    int         incomeUp            = 10;
    float       incomeRate          = 3.0f;
    int         incomeAmountUpCost  = 100;
    int         incomeSpeedUpCost   = 100;
}
