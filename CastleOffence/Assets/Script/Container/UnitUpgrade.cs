using UnityEngine;
using System.Collections;

public class UnitUpgrade : MonoBehaviour
{
    UILabel         _label  = null;
    int             _cost = 0;


    void Start()
    {
        _cost = 100;

        _label = transform.FindChild("Cost").GetComponent<UILabel>();
        _label.text = _cost.ToString();
        _label.depth = 11;

        UIEventListener.Get(gameObject).onClick += onClick;
        UIEventListener.Get(gameObject).onPress += onPress;
    }


    void onClick(GameObject sender)
    {
        var player = GameManager.instance.mPlayer;
        if (player.Purchase(_cost))
        {
            _cost += _cost * 2;
            _label.text = _cost.ToString();




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
}
