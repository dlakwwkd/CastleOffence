using UnityEngine;

public class ItemButton : UIButton
{
    PlayerStatus    _player = null;
    DragDropItem    _item   = null;
    int             _cost   = 0;


    protected override void OnInit()
    {
        _player = GameManager.instance.mPlayer;
        _item = GetComponent<DragDropItem>();
        _cost = (_item.prefab.GetComponent<ObjectStatus>().cost * (int)(_item.xSize * _item.ySize));
        tweenTarget = gameObject;
        base.OnInit();
    }
    protected override void OnClick()
    {
        if(_player.Purchase(_cost))
        {
            _item.Purchase();
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
}
