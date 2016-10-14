using UnityEngine;

public class ItemButton : UIButton
{
    //-----------------------------------------------------------------------------------
    // handler functions
    protected override void OnInit()
    {
        player = GameManager.instance.player;
        item = GetComponent<DragDropItem>();
        cost = (item.Prefab.GetComponent<ObjectStatus>().Cost * (int)(item.XSize * item.YSize));
        tweenTarget = gameObject;
        base.OnInit();
    }

    protected override void OnClick()
    {
        if(player.Purchase(cost))
        {
            item.Purchase();
            AudioManager.instance.PlayPurchaseItem();
        }
        else
            AudioManager.instance.PlayPurchaseFail();
        base.OnClick();
    }

    protected override void OnPress(bool isDown)
    {
        if (isDown)
            Camera.main.GetComponent<CameraMove>().Lock();
        else
            Camera.main.GetComponent<CameraMove>().UnLock();
        base.OnPress(isDown);
    }

    //-----------------------------------------------------------------------------------
    // private field
    PlayerStatus    player  = null;
    DragDropItem    item    = null;
    int             cost    = 0;
}
