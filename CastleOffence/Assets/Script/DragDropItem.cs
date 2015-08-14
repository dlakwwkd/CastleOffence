using UnityEngine;

public class DragDropItem : UIDragDropItem
{
	public GameObject   prefab  = null;
    public float        xSize   = 1.0f;
    public float        ySize   = 1.0f;

    GameObject _obj = null;

    protected override void OnDragDropStart()
    {
        if (mDragScrollView != null)
            mDragScrollView.enabled = false;

        mRoot = NGUITools.FindInParents<UIRoot>(mTrans.parent);

        _obj = ObjectManager.instance.Assign(prefab.name);
        _obj.transform.localScale = new Vector3(xSize, ySize, 1.0f);
        var body = _obj.GetComponent<Rigidbody2D>();
        body.simulated = false;
    }

    protected override void OnDragDropMove(Vector2 delta)
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0.0f;
        _obj.transform.position = pos;
    }

    protected override void OnDragDropRelease(GameObject surface)
	{
        if (mDragScrollView != null)
            StartCoroutine(EnableDragScrollView());

        var body = _obj.GetComponent<Rigidbody2D>();
        body.simulated = true;
        _obj = null;
    }
}
