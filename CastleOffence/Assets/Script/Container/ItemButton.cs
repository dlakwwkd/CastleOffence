using UnityEngine;
using System.Collections;

public class ItemButton : UIButton
{
    DragDropItem _item = null;

    protected override void OnInit()
    {
        _item = GetComponent<DragDropItem>();
        tweenTarget = gameObject;
        base.OnInit();
    }

    protected override void OnClick()
    {
        _item.Purchase();
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
