using UnityEngine;
using System.Collections;

public class UnitUpgrade : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        cost = 100;

        label = transform.FindChild("Cost").GetComponent<UILabel>();
        label.text = cost.ToString();
        label.depth = 11;

        UIEventListener.Get(gameObject).onClick += onClick;
        UIEventListener.Get(gameObject).onPress += onPress;
    }

    void onClick(GameObject sender)
    {
        var player = GameManager.instance.player;
        if (player.Purchase(cost))
        {
            cost += cost * 2;
            label.text = cost.ToString();
            AudioManager.instance.PlayPurchaseUnit();
        }
        else
            AudioManager.instance.PlayPurchaseFail();
    }

    void onPress(GameObject sender, bool isDown)
    {
        if (isDown)
            Camera.main.GetComponent<CameraMove>().Lock();
        else
            Camera.main.GetComponent<CameraMove>().UnLock();
    }

    //-----------------------------------------------------------------------------------
    // private field
    UILabel label   = null;
    int     cost    = 0;
}
